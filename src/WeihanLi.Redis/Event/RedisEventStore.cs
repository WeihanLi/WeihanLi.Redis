using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WeihanLi.Common.Event;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public class EventStoreInRedis : IEventStore
    {
        protected readonly string EventsCacheKey;
        protected readonly ILogger Logger;

        private readonly IRedisWrapper Wrapper;

        public EventStoreInRedis(ILogger<EventStoreInRedis> logger)
        {
            Logger = logger;
            Wrapper = new RedisWrapper(RedisConstants.EventStorePrefix);

            EventsCacheKey = RedisManager.RedisConfiguration.EventStoreCacheKey;
        }

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            var handlerType = typeof(TEventHandler);
            if (Wrapper.Database.HashExists(EventsCacheKey, eventKey))
            {
                var handlers = Wrapper.Unwrap<HashSet<Type>>(Wrapper.Database.HashGet(EventsCacheKey, eventKey));

                if (handlers.Contains(handlerType))
                {
                    return false;
                }
                handlers.Add(handlerType);
                Wrapper.Database.HashSet(EventsCacheKey, eventKey, Wrapper.Wrap(handlers));
                return true;
            }
            else
            {
                return Wrapper.Database.HashSet(EventsCacheKey, eventKey, Wrapper.Wrap(new HashSet<Type> { handlerType }), StackExchange.Redis.When.NotExists);
            }
        }

        public bool Clear()
        {
            return Wrapper.Database.KeyDelete(EventsCacheKey);
        }

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : IEventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return Wrapper.Unwrap<HashSet<Type>>(Wrapper.Database.HashGet(EventsCacheKey, eventKey));
        }

        public string GetEventKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : IEventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return Wrapper.Database.HashExists(EventsCacheKey, eventKey);
        }

        public bool RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            var handlerType = typeof(TEventHandler);

            if (!Wrapper.Database.HashExists(EventsCacheKey, eventKey))
            {
                return false;
            }

            var handlers = Wrapper.Unwrap<HashSet<Type>>(Wrapper.Database.HashGet(EventsCacheKey, eventKey));

            if (!handlers.Contains(handlerType))
            {
                return false;
            }

            handlers.Remove(handlerType);
            Wrapper.Database.HashSet(EventsCacheKey, eventKey, Wrapper.Wrap(handlers));
            return true;
        }
    }
}
