using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;
using WeihanLi.Redis.Event;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class RedisEventBusTest
    {
        public static int counter = 0;
        private readonly IServiceProvider _serviceProvider;

        public RedisEventBusTest()
        {
            var dbIndex = 7;

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddRedisConfig(config =>
            {
                //
                config.RedisServers = new[]
                {
                   new RedisServerConfiguration("127.0.0.1", 6379),
                };
                config.CachePrefix = "WeihanLi.Redis.UnitTest";
                config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
                config.ClientName = "WeihanLi.Redis.UnitTest";

                config.EnableCompress = false;
                config.DefaultDatabase = dbIndex;
            });

            var counter2EventHandler = DelegateEventHandler.FromAction<CounterEvent2>(@event =>
                Console.WriteLine($"{@event.ToJson()}")
            );

            serviceCollection.AddSingleton(counter2EventHandler);

            serviceCollection.AddEvents()
                .UseRedisEventBus()
                .AddEventHandler<CounterEvent, CounterEventHandler>()
                .AddEventHandler<CounterEvent, CounterEventHandler2>()
                // .AddEventHandler<CounterEvent2, DelegateEventHandler<CounterEvent2>>()
                ;

            serviceCollection.AddSingleton<IEventHandler<CounterEvent2>>(counter2EventHandler);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact(Skip = "RedisEventBus")]
        public async Task MainTest()
        {
            var eventBus = _serviceProvider.GetRequiredService<IEventBus>();

            try
            {
                await eventBus.SubscribeAsync<CounterEvent, CounterEventHandler>();
                await eventBus.SubscribeAsync<CounterEvent, CounterEventHandler2>();

                Assert.True(await eventBus.PublishAsync(new CounterEvent { Counter = 123 }));

                await eventBus.SubscribeAsync<CounterEvent2, DelegateEventHandler<CounterEvent2>>();

                Assert.True(await eventBus.PublishAsync(new CounterEvent2 { Counter = 123 }));

                await Task.Delay(15 * 1000);
                Assert.Equal(2, counter);

                await eventBus.UnSubscribeAsync<CounterEvent, CounterEventHandler2>();
                await eventBus.PublishAsync(new CounterEvent { Counter = 123 });

                await Task.Delay(10 * 1000);
                Assert.Equal(3, counter);
            }
            finally
            {
                await Task.WhenAll(
                    eventBus.UnSubscribeAsync<CounterEvent, CounterEventHandler>(),
                    eventBus.UnSubscribeAsync<CounterEvent, CounterEventHandler2>(),
                    eventBus.UnSubscribeAsync<CounterEvent2, DelegateEventHandler<CounterEvent2>>()
                    );
            }
        }

        private class CounterEvent : EventBase
        {
            public int Counter { get; set; }
        }

        private class CounterEvent2 : EventBase
        {
            public int Counter { get; set; }
        }

        private class CounterEventHandler : EventHandlerBase<CounterEvent>
        {
            public override Task Handle(CounterEvent @event)
            {
                DependencyResolver.Current.ResolveService<ILogger<CounterEventHandler>>().Info($"Event:{@event.ToJson()}, HandlerType:{GetType().FullName}");
                Interlocked.Increment(ref counter);
                return Task.CompletedTask;
            }
        }

        private class CounterEventHandler2 : EventHandlerBase<CounterEvent>
        {
            public override Task Handle(CounterEvent @event)
            {
                DependencyResolver.Current.ResolveService<ILogger<CounterEventHandler>>().Info($"Event:{@event.ToJson()}, HandlerType:{GetType().FullName}");
                Interlocked.Increment(ref counter);

                return Task.CompletedTask;
            }
        }
    }
}
