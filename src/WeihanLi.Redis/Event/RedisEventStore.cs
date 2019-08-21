using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WeihanLi.Common.Event;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public class EventStoreInRedis : IEventStore
    {
        private readonly string _eventsCacheKey;
        private readonly ILogger _logger;

        private readonly IRedisWrapper _wrapper;

        public EventStoreInRedis(ILogger<EventStoreInRedis> logger)
        {
            _logger = logger;
            _wrapper = new RedisWrapper(RedisConstants.EventStorePrefix);

            _eventsCacheKey = RedisManager.RedisConfiguration.EventStoreCacheKey;
        }

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            var handlerType = typeof(TEventHandler);
            if (_wrapper.Database.HashExists(_eventsCacheKey, eventKey))
            {
                using (var redLock = RedisManager.GetRedLockClient($"eventStore_{eventKey}", 10 * 1000 / RedisManager.RedisConfiguration.LockRetryDelay))
                {
                    if (redLock.TryLock())
                    {
                        var handlers = _wrapper.Unwrap<HashSet<Type>>(_wrapper.Database.HashGet(_eventsCacheKey, eventKey));

                        if (handlers.Contains(handlerType))
                        {
                            return false;
                        }
                        handlers.Add(handlerType);
                        _wrapper.Database.HashSet(_eventsCacheKey, eventKey, _wrapper.Wrap(handlers));
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return _wrapper.Database.HashSet(_eventsCacheKey, eventKey, _wrapper.Wrap(new HashSet<Type> { handlerType }), StackExchange.Redis.When.NotExists);
            }
        }

        public bool Clear()
        {
            return _wrapper.Database.KeyDelete(_eventsCacheKey);
        }

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : IEventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return _wrapper.Unwrap<HashSet<Type>>(_wrapper.Database.HashGet(_eventsCacheKey, eventKey));
        }

        public string GetEventKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : IEventBase
        {
            var eventKey = GetEventKey<TEvent>();
            return _wrapper.Database.HashExists(_eventsCacheKey, eventKey);
        }

        public bool RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventKey = GetEventKey<TEvent>();
            var handlerType = typeof(TEventHandler);

            if (!_wrapper.Database.HashExists(_eventsCacheKey, eventKey))
            {
                return false;
            }

            using (var redLock = RedisManager.GetRedLockClient($"eventStore_{eventKey}", 10 * 1000 / RedisManager.RedisConfiguration.LockRetryDelay))
            {
                if (redLock.TryLock())
                {
                    var handlers = _wrapper.Unwrap<HashSet<Type>>(_wrapper.Database.HashGet(_eventsCacheKey, eventKey));

                    if (!handlers.Contains(handlerType))
                    {
                        return false;
                    }

                    handlers.Remove(handlerType);
                    _wrapper.Database.HashSet(_eventsCacheKey, eventKey, _wrapper.Wrap(handlers));
                    return true;
                }
            }

            return false;
        }
    }
}
