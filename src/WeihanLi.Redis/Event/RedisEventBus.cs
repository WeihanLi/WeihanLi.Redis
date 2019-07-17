using StackExchange.Redis;
using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public class RedisEventBus : IEventBus
    {
        private readonly IEventStore _eventStore;
        private readonly ISubscriber _subscriber;

        public RedisEventBus(IEventStore eventStore, IConnectionMultiplexer connectionMultiplexer)
        {
            _eventStore = eventStore;
            _subscriber = connectionMultiplexer.GetSubscriber();
        }

        private string GetChannelName<TEvent>() where TEvent : EventBase
        {
            var eventKey = _eventStore.GetEventKey<TEvent>();
            var channelName = $"{RedisManager.RedisConfiguration.EventBusChannelPrefix}{RedisManager.RedisConfiguration.KeySeparator}{eventKey}";

            return channelName;
        }

        public bool Publish<TEvent>(TEvent @event) where TEvent : EventBase
        {
            var channelName = GetChannelName<TEvent>();
            var result = _subscriber.Publish(channelName, @event.ToJson());
            return result > 0;
        }

        public bool Subscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var channelName = GetChannelName<TEvent>();
            _subscriber.Subscribe(channelName, async (channel, eventMessage) =>
           {
               var eventData = eventMessage.ToString().JsonToType<TEvent>();
               if (DependencyResolver.Current.TryResolveService<TEventHandler>(out var handler))
               {
                   await handler.Handle(eventData).ConfigureAwait(false);
               }
           });

            return _eventStore.AddSubscription<TEvent, TEventHandler>();
        }

        public bool Unsubscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var channelName = GetChannelName<TEvent>();
            _subscriber.Unsubscribe(channelName);

            return _eventStore.RemoveSubscription<TEvent, TEventHandler>();
        }
    }
}
