using System;
using System.Collections.Generic;
using WeihanLi.Common.Event;

namespace WeihanLi.Redis.Event
{
    public class RedisEventStore : IEventStore
    {
        public bool IsEmpty => throw new NotImplementedException();

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public bool Clear()
        {
            throw new NotImplementedException();
        }

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : EventBase
        {
            throw new NotImplementedException();
        }

        public string GetEventKey<TEvent>()
        {
            throw new NotImplementedException();
        }

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : EventBase
        {
            throw new NotImplementedException();
        }

        public bool RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }
    }
}
