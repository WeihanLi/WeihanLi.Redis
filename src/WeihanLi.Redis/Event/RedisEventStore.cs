using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
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

        public async Task<int> SaveEventsAsync(ICollection<IEvent> events)
        {
            if (events.IsNullOrEmpty())
                return 0;

            await _database.HashSetAsync(_eventsCacheKey,
                events.Select(e => new HashEntry(e.Properties.EventId, e.ToJson()))
                    .ToArray()
                );

            return events.Count;
        }
        
        public async Task<int> DeleteEventsAsync(ICollection<string> events)
        {
            if (events.IsNullOrEmpty())
                return 0;

            await _database.HashDeleteAsync(_eventsCacheKey,
                events.Select(x => (RedisValue)x)
                    .ToArray()
                );

            return events.Count;
        }
    }
}
