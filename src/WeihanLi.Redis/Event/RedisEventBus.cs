using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public class RedisEventBus : IEventBus
    {
        private readonly ISubscriber _subscriber;
        private readonly IEventSubscriptionManager _subscriptionManager;
        private readonly IServiceProvider _serviceProvider;

        public RedisEventBus(IEventSubscriptionManager eventSubscriptionManager, IConnectionMultiplexer connectionMultiplexer, IServiceProvider serviceProvider)
        {
            _subscriptionManager = eventSubscriptionManager;
            _serviceProvider = serviceProvider;
            _subscriber = connectionMultiplexer.GetSubscriber();
        }

        private string GetChannelPrefix<TEvent>() where TEvent : class, IEventBase
        {
            var eventKey = typeof(TEvent).FullName;
            var channelPrefix =
                $"{RedisManager.RedisConfiguration.EventBusChannelPrefix}{RedisManager.RedisConfiguration.KeySeparator}{eventKey}{RedisManager.RedisConfiguration.KeySeparator}";
            return channelPrefix;
        }

        private string GetChannelName<TEvent, TEventHandler>() where TEvent : class, IEventBase
            where TEventHandler : IEventHandler<TEvent>
            => GetChannelName<TEvent>(typeof(TEventHandler));

        private string GetChannelName<TEvent>(Type eventHandlerType) where TEvent : class, IEventBase
        {
            var channelPrefix = GetChannelPrefix<TEvent>();
            var channelName = $"{channelPrefix}{eventHandlerType.FullName}";

            return channelName;
        }

        public bool Publish<TEvent>(TEvent @event) where TEvent : class, IEventBase
        {
            var handlerTypes = _subscriptionManager.GetEventHandlerTypes<TEvent>();
            if (handlerTypes == null || handlerTypes.Count == 0)
            {
                return false;
            }

            var eventData = @event.ToJson();
            foreach (var handlerType in handlerTypes)
            {
                var handlerChannelName = GetChannelName<TEvent>(handlerType);
                _subscriber.Publish(handlerChannelName, eventData);
            }

            return true;
        }

        public async Task<bool> PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEventBase
        {
            var handlerTypes = _subscriptionManager.GetEventHandlerTypes<TEvent>();
            if (handlerTypes == null || handlerTypes.Count == 0)
            {
                return false;
            }

            var eventData = @event.ToJson();
            await Task.WhenAll(handlerTypes.Select(handlerType =>
            {
                var handlerChannelName = GetChannelName<TEvent>(handlerType);
                return _subscriber.PublishAsync(handlerChannelName, eventData);
            }));

            return true;
        }

        public bool Subscribe<TEvent, TEventHandler>()
            where TEvent : class, IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            _subscriptionManager.Subscribe<TEvent, TEventHandler>();

            var channelName = GetChannelName<TEvent, TEventHandler>();

            _subscriber.Subscribe(channelName, async (channel, eventMessage) =>
            {
                var eventData = eventMessage.ToString().JsonToObject<TEvent>();
                var handler = _serviceProvider.GetServiceOrCreateInstance<TEventHandler>();
                if (null != handler)
                {
                    await handler.Handle(eventData).ConfigureAwait(false);
                }
            });
            return true;
        }

        public async Task<bool> SubscribeAsync<TEvent, TEventHandler>() where TEvent : class, IEventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var channelName = GetChannelName<TEvent, TEventHandler>();
            await Task.WhenAll(
                _subscriptionManager.SubscribeAsync<TEvent, TEventHandler>(),
                _subscriber.SubscribeAsync(channelName, async (channel, eventMessage) =>
                    {
                        var eventData = eventMessage.ToString().JsonToObject<TEvent>();
                        var handler = _serviceProvider.GetServiceOrCreateInstance<TEventHandler>();
                        if (null != handler)
                        {
                            await handler.Handle(eventData).ConfigureAwait(false);
                        }
                    })
                );
            return true;
        }

        public bool UnSubscribe<TEvent, TEventHandler>() where TEvent : class, IEventBase where TEventHandler : IEventHandler<TEvent> => _subscriptionManager.UnSubscribe<TEvent, TEventHandler>();

        public Task<bool> UnSubscribeAsync<TEvent, TEventHandler>() where TEvent : class, IEventBase where TEventHandler : IEventHandler<TEvent> => _subscriptionManager.UnSubscribeAsync<TEvent, TEventHandler>();

        public ICollection<Type> GetEventHandlerTypes<TEvent>() where TEvent : class, IEventBase => _subscriptionManager.GetEventHandlerTypes<TEvent>();
    }
}
