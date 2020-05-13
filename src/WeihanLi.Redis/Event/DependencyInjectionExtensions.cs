using Microsoft.Extensions.DependencyInjection;
using System;
using WeihanLi.Common.Event;

namespace WeihanLi.Redis.Event
{
    public static class DependencyInjectionExtensions
    {
        public static IEventBuilder UseRedisBus(this IEventBuilder eventBuilder)
        {
            if (null == eventBuilder)
            {
                throw new ArgumentNullException(nameof(eventBuilder));
            }

            eventBuilder.Services.AddSingleton<IEventBus, RedisEventBus>();
            return eventBuilder;
        }

        public static IEventBuilder UseRedisQueue(this IEventBuilder eventBuilder)
        {
            if (null == eventBuilder)
            {
                throw new ArgumentNullException(nameof(eventBuilder));
            }

            eventBuilder.Services.AddSingleton<IEventQueue, RedisEventQueue>();
            return eventBuilder;
        }

        public static IEventBuilder UseRedisStore(this IEventBuilder eventBuilder)
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
