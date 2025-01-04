using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
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

        public async Task<bool> EnqueueAsync<TEvent>(string queueName, TEvent @event, EventProperties properties = null)
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
                _logger.LogError(e, "enqueue event exception, queueName: {QueueName}, eventId:{EventId}",
                    queueName, (@event as IEvent)?.Properties.EventId);
                return false;
            }
            return true;
        }

        public async Task<IEvent<TEvent>> DequeueAsync<TEvent>(string queueName)
        {
            ArgumentNullException.ThrowIfNull(queueName);

            var result = await _database.ListLeftPopAsync(GetQueueCacheKey(queueName));
            if (result.HasValue)
            {
                try
                {
                    var @event = result.ToString().ToEvent<IEvent<TEvent>>();
                    return @event;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"dequeue event exception, queueName:{queueName}");
                }
            }

            return null;
        }

        public async IAsyncEnumerable<IEvent> ReadAllAsync(
            string queueName,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
            )
        {
            ArgumentNullException.ThrowIfNull(queueName);
            var queueKey = GetQueueCacheKey(queueName);
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _database.ListLeftPopAsync(queueKey);
                if (result.HasValue)
                {
                    var @event = result.ToString().ToEvent();
                    yield return @event;
                }
                else
                {
                    await Task.Delay(200, cancellationToken);
                }
            }
        }

        public ICollection<string> GetQueues() => _database.SetMembers(_cacheKey).ToStringArray();

        public async Task<ICollection<string>> GetQueuesAsync() =>
            await _database.SetMembersAsync(_cacheKey)
            .ContinueWith(r => r.Result.ToStringArray());
    }
}
