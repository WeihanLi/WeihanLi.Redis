using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Event;

namespace WeihanLi.Redis.Event
{
    public sealed class RedisEventQueue : IEventQueue
    {
        private readonly IDatabase _database;
        private readonly ILogger _logger;
        private readonly string _cacheKey;

        public RedisEventQueue(IDatabase database, ILogger<RedisEventQueue> logger)
        {
            _database = database;
            _logger = logger;
            _cacheKey = RedisManager.RedisConfiguration.EventQueueStorePrefix;
        }

        private string GetQueueCacheKey(string queueName) =>
            $"{_cacheKey}{RedisManager.RedisConfiguration.KeySeparator}{queueName}";

        public bool Enqueue<TEvent>(string queueName, TEvent @event)
            where TEvent : class, IEventBase
        {
            if (null == queueName)
                throw new ArgumentNullException(nameof(queueName));

            if (null == @event)
                return false;

            try
            {
                _database.SetAdd(_cacheKey, queueName);

                var eventMsg = @event.ToEventMsg();
                _database.ListRightPush(GetQueueCacheKey(queueName), eventMsg);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"enqueue event exception, queueName{queueName}, eventId:{@event.EventId}");
                return false;
            }
            return true;
        }

        public async Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event)
            where TEvent : class, IEventBase
        {
            if (null == queueName)
                throw new ArgumentNullException(nameof(queueName));

            if (null == @event)
                return false;

            try
            {
                var eventMsg = @event.ToEventMsg();

                await Task.WhenAll(
                    _database.SetAddAsync(_cacheKey, queueName),
                    _database.ListRightPushAsync(GetQueueCacheKey(queueName), eventMsg)
                    );
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"enqueue event exception, queueName{queueName}, eventId:{@event.EventId}");
                return false;
            }
            return true;
        }

        public IEventBase Dequeue(string queueName)
        {
            if (null == queueName)
                throw new ArgumentNullException(nameof(queueName));

            var result = _database.ListLeftPop(GetQueueCacheKey(queueName));
            if (result.HasValue)
            {
                IEventBase @event = null;
                try
                {
                    @event = result.ToString().ToEvent();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"dequeue event exception, queueName:{queueName}");
                }

                return @event;
            }

            return null;
        }

        public async Task<IEventBase> DequeueAsync(string queueName)
        {
            if (null == queueName)
                throw new ArgumentNullException(nameof(queueName));

            var result = await _database.ListLeftPopAsync(GetQueueCacheKey(queueName));
            if (result.HasValue)
            {
                IEventBase @event = null;
                try
                {
                    @event = result.ToString().ToEvent();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"dequeue event exception, queueName:{queueName}");
                }

                return @event;
            }

            return null;
        }

        public ICollection<string> GetQueues() => _database.SetMembers(_cacheKey).ToStringArray();

        public async Task<ICollection<string>> GetQueuesAsync() =>
            await _database.SetMembersAsync(_cacheKey)
            .ContinueWith(r => r.Result.ToStringArray());
    }
}
