using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WeihanLi.Common.Event;

namespace WeihanLi.Redis.Event
{
    public class RedisEventStore : BaseRedisClient, IEventStore
    {
        private const string EventsCacheKey = "Events";

        public RedisEventStore(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<RedisEventStore>(), new RedisWrapper("EventStore"))
        {
        }

        public bool IsEmpty => Wrapper.Database.KeyExists(EventsCacheKey);

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            var handlerType = typeof(TEventHandler);
            if (Wrapper.Database.HashExists(EventsCacheKey, eventKey))
            {
                var handlers = Wrapper.Unwrap<List<Type>>(Wrapper.Database.HashGet(EventsCacheKey, eventKey));

                if (handlers.Contains(handlerType))
                {
                    return false;
                }
                handlers.Add(handlerType);
                return Wrapper.Database.HashSet(EventsCacheKey, eventKey, Wrapper.Wrap(handlers));
            }
            else
            {
                return Wrapper.Database.HashSet(EventsCacheKey, eventKey, Wrapper.Wrap(new List<Type> { handlerType }), StackExchange.Redis.When.NotExists);
            }
        }

        public bool Clear()
        {
            return Wrapper.Database.KeyDelete(EventsCacheKey);
        }

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : EventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return Wrapper.Unwrap<List<Type>>(Wrapper.Database.HashGet(EventsCacheKey, eventKey));
        }

        public string GetEventKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : EventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return Wrapper.Database.HashExists(EventsCacheKey, eventKey);
        }

        public bool RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            var handlerType = typeof(TEventHandler);
            if (Wrapper.Database.HashExists(EventsCacheKey, eventKey))
            {
                var handlers = Wrapper.Unwrap<List<Type>>(Wrapper.Database.HashGet(EventsCacheKey, eventKey));

                if (!handlers.Contains(handlerType))
                {
                    return false;
                }
                handlers.Remove(handlerType);
                return Wrapper.Database.HashSet(EventsCacheKey, eventKey, Wrapper.Wrap(handlers));
            }
            else
            {
                return false;
            }
        }
    }
}
