using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WeihanLi.Common.Event;
using WeihanLi.Redis.Internals;

namespace WeihanLi.Redis.Event
{
    public class EventStoreInRedis : BaseRedisClient, IEventStore
    {
        private const string EventsCacheKey = "Events";

        public EventStoreInRedis(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<EventStoreInRedis>(), new RedisWrapper(RedisConstants.EventStorePrefix))
        {
        }

        public bool IsEmpty => Wrapper.Database.KeyExists(Wrapper.GetRealKey(EventsCacheKey));

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            var handlerType = typeof(TEventHandler);
            if (Wrapper.Database.HashExists(Wrapper.GetRealKey(EventsCacheKey), eventKey))
            {
                var handlers = Wrapper.Unwrap<List<Type>>(Wrapper.Database.HashGet(Wrapper.GetRealKey(EventsCacheKey), eventKey));

                if (handlers.Contains(handlerType))
                {
                    return false;
                }
                handlers.Add(handlerType);
                return Wrapper.Database.HashSet(Wrapper.GetRealKey(EventsCacheKey), eventKey, Wrapper.Wrap(handlers));
            }
            else
            {
                return Wrapper.Database.HashSet(Wrapper.GetRealKey(EventsCacheKey), eventKey, Wrapper.Wrap(new List<Type> { handlerType }), StackExchange.Redis.When.NotExists);
            }
        }

        public bool Clear()
        {
            return Wrapper.Database.KeyDelete(Wrapper.GetRealKey(EventsCacheKey));
        }

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : EventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return Wrapper.Unwrap<List<Type>>(Wrapper.Database.HashGet(Wrapper.GetRealKey(EventsCacheKey), eventKey));
        }

        public string GetEventKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : EventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return Wrapper.Database.HashExists(Wrapper.GetRealKey(EventsCacheKey), eventKey);
        }

        public bool RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            var handlerType = typeof(TEventHandler);
            if (Wrapper.Database.HashExists(Wrapper.GetRealKey(EventsCacheKey), eventKey))
            {
                var handlers = Wrapper.Unwrap<List<Type>>(Wrapper.Database.HashGet(Wrapper.GetRealKey(EventsCacheKey), eventKey));

                if (!handlers.Contains(handlerType))
                {
                    return false;
                }
                handlers.Remove(handlerType);
                return Wrapper.Database.HashSet(Wrapper.GetRealKey(EventsCacheKey), eventKey, Wrapper.Wrap(handlers));
            }
            else
            {
                return false;
            }
        }
    }
}
