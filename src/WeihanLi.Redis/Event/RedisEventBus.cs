using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Redis.Event
{
    public sealed class RedisEventBus : IEventBus
    {
        private readonly ISubscriber _subscriber;
        private readonly IEventSubscriptionManager _subscriptionManager;
        private readonly IServiceProvider _serviceProvider;

        public RedisEventBus(IEventSubscriptionManager eventSubscriptionManager, ISubscriber subscriber, IServiceProvider serviceProvider)
        {
            _subscriptionManager = eventSubscriptionManager;
            _subscriber = subscriber;
            _serviceProvider = serviceProvider;
        }

        private static string GetChannelPrefix<TEvent>() where TEvent : class, IEventBase
        {
            var eventKey = typeof(TEvent).FullName;
            var channelPrefix =
                $"{RedisManager.RedisConfiguration.EventBusChannelPrefix}{RedisManager.RedisConfiguration.KeySeparator}{eventKey}{RedisManager.RedisConfiguration.KeySeparator}";
            return channelPrefix;
        }

        private static string GetChannelPrefix(Type eventType)
        {
            var eventKey = eventType.FullName;
            var channelPrefix =
                $"{RedisManager.RedisConfiguration.EventBusChannelPrefix}{RedisManager.RedisConfiguration.KeySeparator}{eventKey}{RedisManager.RedisConfiguration.KeySeparator}";
            return channelPrefix;
        }

        private static string GetChannelName<TEvent>(Type eventHandlerType) where TEvent : class, IEventBase
        {
            var channelPrefix = GetChannelPrefix<TEvent>();
            var channelName = $"{channelPrefix}{eventHandlerType.FullName}";

            return channelName;
        }

        private static string GetChannelName(Type eventType, Type eventHandlerType)
        {
            var channelPrefix = GetChannelPrefix(eventType);
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

            var eventData = @event.ToEventMsg();
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

            var eventData = @event.ToEventMsg();
            await Task.WhenAll(handlerTypes.Select(handlerType =>
            {
                var handlerChannelName = GetChannelName<TEvent>(handlerType);
                return _subscriber.PublishAsync(handlerChannelName, eventData);
            }));

            return true;
        }

        public bool Subscribe(Type eventType, Type eventHandlerType)
        {
            var channelName = GetChannelName(eventType, eventHandlerType);
            _subscriber.Subscribe(channelName, async (channel, eventMessage) =>
            {
                var eventData = eventMessage.ToString().ToEvent();
                var handler = (IEventHandler)_serviceProvider.GetServiceOrCreateInstance(eventHandlerType);
                if (null != handler)
                {
                    await handler.Handle(eventData).ConfigureAwait(false);
                }
            });
            return _subscriptionManager.Subscribe(eventType, eventHandlerType);
        }

        public async Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType)
        {
            var channelName = GetChannelName(eventType, eventHandlerType);
            await Task.WhenAll(
                _subscriptionManager.SubscribeAsync(eventType, eventHandlerType),
                _subscriber.SubscribeAsync(channelName, async (channel, eventMessage) =>
                {
                    var eventData = eventMessage.ToString().ToEvent();
                    var handler = (IEventHandler)_serviceProvider.GetServiceOrCreateInstance(eventHandlerType);
                    if (null != handler)
                    {
                        await handler.Handle(eventData).ConfigureAwait(false);
                    }
                })
            );
            return true;
        }

        public bool UnSubscribe(Type eventType, Type eventHandlerType)
        {
            var channelName = GetChannelName(eventType, eventHandlerType);
            _subscriber.Unsubscribe(channelName);
            return _subscriptionManager.UnSubscribe(eventType, eventHandlerType);
        }

        public async Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType)
        {
            var channelName = GetChannelName(eventType, eventHandlerType);
            await _subscriber.UnsubscribeAsync(channelName);
            return await _subscriptionManager.UnSubscribeAsync(eventType, eventHandlerType);
        }
    }
}
