using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
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

        public int SaveEvents(params IEventBase[] events)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveEventsAsync(params IEventBase[] events)
        {
            throw new NotImplementedException();
        }

        public int DeleteEvents(params string[] events)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteEventsAsync(params string[] events)
        {
            throw new NotImplementedException();
        }
    }
}
