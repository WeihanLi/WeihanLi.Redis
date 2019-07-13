using StackExchange.Redis;
using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Extensions;

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
            var channelName = $"{RedisManager.RedisConfiguration.ChannelPrefix}{RedisManager.RedisConfiguration.KeySeparator}{eventKey}";

            return channelName;
        }

        public bool Publish<TEvent>(TEvent @event) where TEvent : EventBase
        {
            var channelName = GetChannelName<TEvent>();
            var result = _subscriber.Publish(channel: channelName, @event.ToJson());
            return result > 0;
        }

        public bool Subscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var channelName = GetChannelName<TEvent>();
            _subscriber.Subscribe(channelName, (channel, eventMessage) =>
            {
                var eventData = eventMessage.ToString().JsonToType<TEvent>();
                if (DependencyResolver.Current.TryResolveService<TEventHandler>(out var handler))
                {
                    handler.Handle(eventData).ConfigureAwait(false);
                }
            });

            _eventStore.AddSubscription<TEvent, TEventHandler>();
            return true;
        }

        public bool Unsubscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var channelName = GetChannelName<TEvent>();
            _subscriber.Unsubscribe(channelName);

            _eventStore.RemoveSubscription<TEvent, TEventHandler>();
            return true;
        }
    }
}
