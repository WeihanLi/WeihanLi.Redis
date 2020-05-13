using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using WeihanLi.Common.Event;
using WeihanLi.Extensions;

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

        public bool Enqueue<TEvent>(string queueName, TEvent @event) where TEvent : IEventBase
        {
            if (null == queueName)
                throw new ArgumentNullException(nameof(queueName));

            if (null == @event)
                return false;

            try
            {
                _database.SetAdd(_cacheKey, queueName);

                // ensure event type
                var jObj = JObject.FromObject(@event);
                if (!jObj.ContainsKey("Type"))
                {
                    jObj["Type"] = JToken.FromObject(@event.GetType());
                }

                _database.ListRightPush(GetQueueCacheKey(queueName), jObj.ToJson());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"enqueue event exception, queueName{queueName}, eventId:{@event.EventId}");
                return false;
            }
            return true;
        }

        public bool TryDequeue(string queueName, out IEventBase @event)
        {
            if (null == queueName)
                throw new ArgumentNullException(nameof(queueName));

            @event = null;

            var result = _database.ListLeftPop(GetQueueCacheKey(queueName));
            if (result.HasValue)
            {
                try
                {
                    var jObj = JObject.Parse(result.ToString());
                    var eventType = jObj["Type"].ToObject<Type>();
                    @event = (IEventBase)jObj.ToObject(eventType, JsonSerializer.Create(JsonSerializeExtension.DefaultSerializerSettings));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"dequeue event exception, queueName:{queueName}");
                    return false;
                }

                return true;
            }

            return false;
        }

        public bool TryRemoveQueue(string queueName)
        {
            if (null == queueName)
                throw new ArgumentNullException(nameof(queueName));

            _database.SetRemove(_cacheKey, queueName);
            return _database.KeyDelete(GetQueueCacheKey(queueName));
        }

        public ICollection<string> Queues => _database.SetMembers(_cacheKey).ToStringArray();
    }
}
