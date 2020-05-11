using System;
using System.Collections.Generic;
using WeihanLi.Common.Event;

namespace WeihanLi.Redis.Event
{
    public class RedisEventQueue : IEventQueue
    {
        public bool Enqueue<TEvent>(string queueName, TEvent @event) where TEvent : IEventBase => throw new NotImplementedException();

        public bool TryDequeue(string queueName, out IEventBase @event) => throw new NotImplementedException();

        public bool TryRemoveQueue(string queueName) => throw new NotImplementedException();

        public ICollection<string> Queues { get; }
    }
}
