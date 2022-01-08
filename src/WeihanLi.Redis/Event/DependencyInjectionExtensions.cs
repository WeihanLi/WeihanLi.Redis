using System;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common.Event;

namespace WeihanLi.Redis.Event
{
    public static class DependencyInjectionExtensions
    {
        public static IEventBuilder UseRedisEventBus(this IEventBuilder eventBuilder)
        {
            if (null == eventBuilder)
            {
                throw new ArgumentNullException(nameof(eventBuilder));
            }

            eventBuilder.Services.AddSingleton<IEventBus, RedisEventBus>();
            return eventBuilder;
        }

        public static IEventBuilder UseRedisEventQueue(this IEventBuilder eventBuilder)
        {
            if (null == eventBuilder)
            {
                throw new ArgumentNullException(nameof(eventBuilder));
            }

            eventBuilder.Services.AddSingleton<IEventQueue, RedisEventQueue>();
            return eventBuilder;
        }

        public static IEventBuilder UseRedisEventStore(this IEventBuilder eventBuilder)
        {
            if (null == eventBuilder)
            {
                throw new ArgumentNullException(nameof(eventBuilder));
            }

            eventBuilder.Services.AddSingleton<IEventStore, EventStoreInRedis>();
            return eventBuilder;
        }
    }
}
