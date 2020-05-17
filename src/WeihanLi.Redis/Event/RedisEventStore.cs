using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Event;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public sealed class EventStoreInRedis : IEventStore
    {
        private readonly string _eventsCacheKey;
        private readonly IDatabase _database;

        public EventStoreInRedis(IDatabase database)
        {
            _database = database;
            _eventsCacheKey = RedisManager.RedisConfiguration.EventStoreCacheKey;
        }

        public int SaveEvents(params IEventBase[] events)
        {
            if (null == events || events.Length == 0)
                return 0;

            _database.HashSet(_eventsCacheKey,
                events.Select(e => new HashEntry(e.EventId, e.ToEventMsg())
                    )
                    .ToArray()
                );

            return events.Length;
        }

        public async Task<int> SaveEventsAsync(params IEventBase[] events)
        {
            if (null == events || events.Length == 0)
                return 0;

            await _database.HashSetAsync(_eventsCacheKey,
                events.Select(e => new HashEntry(e.EventId, e.ToEventMsg()))
                    .ToArray()
                );

            return events.Length;
        }

        public int DeleteEvents(params string[] events)
        {
            if (null == events || events.Length == 0)
                return 0;

            _database.HashDelete(_eventsCacheKey,
                events.Select(x => (RedisValue)x)
                    .ToArray()
                );

            return events.Length;
        }

        public async Task<int> DeleteEventsAsync(params string[] events)
        {
            if (null == events || events.Length == 0)
                return 0;

            await _database.HashDeleteAsync(_eventsCacheKey,
                events.Select(x => (RedisValue)x)
                    .ToArray()
                );

            return events.Length;
        }
    }
}
