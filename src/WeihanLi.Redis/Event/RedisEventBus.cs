using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis.Event
{
    public sealed class RedisEventBus : IEventBus
    {
        private readonly ISubscriber _subscriber;
        private readonly IEventSubscriptionManager _subscriptionManager;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IServiceProvider _serviceProvider;

        public RedisEventBus(IEventSubscriptionManager eventSubscriptionManager, IEventHandlerFactory eventHandlerFactory, ISubscriber subscriber, IServiceProvider serviceProvider)
        {
            _subscriptionManager = eventSubscriptionManager;
            _eventHandlerFactory = eventHandlerFactory;
            _subscriber = subscriber;
            _serviceProvider = serviceProvider;
        }

        private static string GetChannelPrefix<TEvent>()
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

        private static string GetChannelName<TEvent>(Type eventHandlerType)
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
        
        public async Task<bool> PublishAsync<TEvent>(TEvent @event, EventProperties eventProperties)
        {
            var handlers = _eventHandlerFactory.GetHandlers<TEvent>();
            if (handlers is not { Count: not 0 })
            {
                return false;
            }

            var eventData = @event.ToEventMsg();
            await Task.WhenAll(handlers.Select(handler =>
            {
                var handlerChannelName = GetChannelName<TEvent>(handler.GetType());
                return _subscriber.PublishAsync(handlerChannelName, eventData);
            }));

            return true;
        }
        
        public async Task<bool> SubscribeAsync(Type eventType, Type eventHandlerType)
        {
            var channelName = GetChannelName(eventType, eventHandlerType);
            await Task.WhenAll(
                _subscriptionManager.SubscribeAsync(eventType, eventHandlerType),
                _subscriber.SubscribeAsync(RedisChannel.Literal(channelName), async (channel, eventMessage) =>
                {
                    var eventData = eventMessage.ToString().ToEvent();
                    var handler = (IEventHandler)_serviceProvider.GetServiceOrCreateInstance(eventHandlerType);
                    if (handler is not null)
                    {
                        await handler.Handle(eventData, eventData is IEvent @event ? @event.Properties : new()).ConfigureAwait(false);
                    }
                })
            );
            return true;
        }

        public async Task<bool> SubscribeAsync<TEvent>(IEventHandler<TEvent> eventHandler)
        {
            var channelName = GetChannelName(typeof(TEvent), eventHandler.GetType());
            await _subscriptionManager.SubscribeAsync(eventHandler);
            await _subscriber.SubscribeAsync(RedisChannel.Literal(channelName), async (channel, eventMessage) =>
            {
                var eventData = eventMessage.ToString().ToEvent();
                await eventHandler.Handle(eventData, eventData is IEvent @event ? @event.Properties : new())
                    .ConfigureAwait(false);
            });
            return true;
        }

        public async Task<bool> UnSubscribeAsync(Type eventType, Type eventHandlerType)
        {
            var channelName = GetChannelName(eventType, eventHandlerType);
            await _subscriber.UnsubscribeAsync(RedisChannel.Literal(channelName));
            return await _subscriptionManager.UnSubscribeAsync(eventType, eventHandlerType);
        }
    }
}
