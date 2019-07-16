using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Extensions;
using WeihanLi.Redis.Event;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class RedisEventBusTest
    {
        public static int counter = 0;

        static RedisEventBusTest()
        {
            var dbIndex = 7;

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddRedisConfig(config =>
            {
                //
                //config.RedisServers = new[]
                //{
                //    new RedisServerConfiguration("127.0.0.1", 6379),
                //};
                config.CachePrefix = "WeihanLi.Redis.UnitTest";
                config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
                config.EnableCompress = false;
                config.DefaultDatabase = dbIndex;
            });
            serviceCollection.AddSingleton<IEventStore, EventStoreInRedis>();
            serviceCollection.AddSingleton<IEventBus, RedisEventBus>();
            serviceCollection.AddSingleton<CounterEventHandler>();
            serviceCollection.AddSingleton<CounterEventHandler2>();

            DependencyResolver.SetDependencyResolver(serviceCollection);
        }

        [Fact]
        public async Task MainTest()
        {
            try
            {
                var eventBus = DependencyResolver.Current.ResolveService<IEventBus>();

                eventBus.Subscribe<CounterEvent, CounterEventHandler>();
                eventBus.Subscribe<CounterEvent, CounterEventHandler2>();
                eventBus.Publish(new CounterEvent { Counter = 123 });

                await Task.Delay(10 * 1000);
                Assert.Equal(2, counter);
            }
            finally
            {
                DependencyResolver.Current.GetRequiredService<IEventStore>()
                    .Clear();
                RedisManager.PubSubClient.UnsubscribeAll();
            }
        }

        private class CounterEvent : EventBase
        {
            public int Counter { get; set; }
        }

        private class CounterEventHandler : IEventHandler<CounterEvent>
        {
            public Task Handle(CounterEvent @event, CancellationToken cancellationToken = default)
            {
                System.Console.WriteLine($"Event:{@event.ToJson()}, HandlerType:{GetType().FullName}");
                Interlocked.Increment(ref counter);
                return Task.CompletedTask;
            }
        }

        private class CounterEventHandler2 : IEventHandler<CounterEvent>
        {
            public Task Handle(CounterEvent @event, CancellationToken cancellationToken = default)
            {
                System.Console.WriteLine($"Event:{@event.ToJson()}, HandlerType:{GetType().FullName}");
                Interlocked.Increment(ref counter);

                return Task.CompletedTask;
            }
        }
    }
}
