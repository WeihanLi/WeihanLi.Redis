using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Event;
using WeihanLi.Extensions;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public sealed class EventStoreInRedis : IEventStore
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
            if (null == events || events.Length == 0)
                return 0;

            _wrapper.Database.HashSet(_eventsCacheKey, events.Select(e => new HashEntry(e.EventId, e.ToJson())).ToArray());

            return events.Length;
        }

        public async Task<int> SaveEventsAsync(params IEventBase[] events)
        {
            if (null == events || events.Length == 0)
                return 0;

            await _wrapper.Database.HashSetAsync(_eventsCacheKey,
                events.Select(e => new HashEntry(e.EventId, e.ToJson())).ToArray()
                );

            return events.Length;
        }

        public int DeleteEvents(params string[] events)
        {
            if (null == events || events.Length == 0)
                return 0;

            _wrapper.Database.HashDelete(_eventsCacheKey, events.Select(x => (RedisValue)x).ToArray());

            return events.Length;
        }

        public async Task<int> DeleteEventsAsync(params string[] events)
        {
            if (null == events || events.Length == 0)
                return 0;

            await _wrapper.Database.HashDeleteAsync(_eventsCacheKey, events.Select(x => (RedisValue)x).ToArray());

            return events.Length;
        }
    }
}
