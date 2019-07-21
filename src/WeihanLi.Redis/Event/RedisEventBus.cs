using System;
using StackExchange.Redis;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public class RedisEventBus : IEventBus
    {
        private readonly IEventStore _eventStore;
        private readonly ISubscriber _subscriber;
        private readonly IServiceProvider _serviceProvider;

        public RedisEventBus(IEventStore eventStore, IConnectionMultiplexer connectionMultiplexer, IServiceProvider serviceProvider)
        {
            _eventStore = eventStore;
            _serviceProvider = serviceProvider;
            _subscriber = connectionMultiplexer.GetSubscriber();
        }

        private string GetChannelName<TEvent, TEventHandler>() where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
            => GetChannelName<TEvent>(typeof(TEventHandler));

        private string GetChannelName<TEvent>(Type eventHandlerType) where TEvent : IEventBase
        {
            var eventKey = _eventStore.GetEventKey<TEvent>();
            var channelName = $"{RedisManager.RedisConfiguration.EventBusChannelPrefix}{RedisManager.RedisConfiguration.KeySeparator}{eventKey}{RedisManager.RedisConfiguration.KeySeparator}{eventHandlerType.FullName}";

            return channelName;
        }

        public bool Publish<TEvent>(TEvent @event) where TEvent : IEventBase
        {
            if (!_eventStore.HasSubscriptionsForEvent<TEvent>())
            {
                return false;
            }
            var handlerTypes = _eventStore.GetEventHandlerTypes<TEvent>();
            var eventData = @event.ToJson();
            foreach (var handlerType in handlerTypes)
            {
                var channelName = GetChannelName<TEvent>(handlerType);
                _subscriber.Publish(channelName, eventData);
            }
            return true;
        }

        public bool Subscribe<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            _eventStore.AddSubscription<TEvent, TEventHandler>();

            var channelName = GetChannelName<TEvent, TEventHandler>();
            if (!_subscriber.IsConnected(channelName))
            {
                _subscriber.Subscribe(channelName, async (channel, eventMessage) =>
                {
                    var eventData = eventMessage.ToString().JsonToType<TEvent>();
                    var handler = _serviceProvider.GetServiceOrCreateInstance<TEventHandler>();
                    if (null != handler)
                    {
                        await handler.Handle(eventData).ConfigureAwait(false);
                    }
                });
                return true;
            }

            return false;
        }

        public bool Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            _eventStore.RemoveSubscription<TEvent, TEventHandler>();

            var channelName = GetChannelName<TEvent, TEventHandler>();
            if (_subscriber.IsConnected(channelName))
            {
                _subscriber.Unsubscribe(channelName);
                return true;
            }
            return false;
        }
    }
}
