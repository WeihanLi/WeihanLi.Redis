using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common;
using WeihanLi.Common.Event;
using WeihanLi.Common.Logging;
using WeihanLi.Common.Logging.Log4Net;
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
                //config.RedisServers = new[]
                //{
                //    new RedisServerConfiguration("127.0.0.1", 6379),
                //};
                config.CachePrefix = "WeihanLi.Redis.UnitTest";
                config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
                config.ClientName = "WeihanLi.Redis.UnitTest";

                config.EnableCompress = false;
                config.DefaultDatabase = dbIndex;
            });
            serviceCollection.AddEvents();
            serviceCollection.AddSingleton<IEventBus, RedisEventBus>();
            serviceCollection.AddSingleton<CounterEventHandler>();
            serviceCollection.AddSingleton<CounterEventHandler2>();

            serviceCollection.AddSingleton(
                DelegateEventHandler.FromAction<CounterEvent2>(@event =>
                    Log4NetHelper.GetLogger("DelegateEventHandler+CounterEvents").Info($"{@event.ToJson()}")
                    )
                );

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceProvider.GetRequiredService<ILoggerFactory>().AddLog4Net();
        }

        [Fact]
        public async Task MainTest()
        {
            var eventBus = _serviceProvider.GetRequiredService<IEventBus>();

            try
            {
                Assert.True(eventBus.Subscribe<CounterEvent, CounterEventHandler>());
                Assert.True(eventBus.Subscribe<CounterEvent, CounterEventHandler2>());

                // Assert.False(eventBus.Subscribe<CounterEvent, CounterEventHandler2>());
                Assert.True(eventBus.Publish(new CounterEvent { Counter = 123 }));

                Assert.True(eventBus.Subscribe<CounterEvent2, DelegateEventHandler<CounterEvent2>>());
                // Assert.False(eventBus.Subscribe<CounterEvent2, DelegateEventHandler<CounterEvent2>>());
                Assert.True(eventBus.Publish(new CounterEvent2 { Counter = 123 }));

                await Task.Delay(15 * 1000);
                Assert.Equal(2, counter);

                eventBus.UnSubscribe<CounterEvent, CounterEventHandler2>();
                eventBus.Publish(new CounterEvent { Counter = 123 });

                await Task.Delay(10 * 1000);
                Assert.Equal(3, counter);
            }
            finally
            {
                eventBus.UnSubscribe<CounterEvent, CounterEventHandler>();
                eventBus.UnSubscribe<CounterEvent, CounterEventHandler2>();
                eventBus.UnSubscribe<CounterEvent2, DelegateEventHandler<CounterEvent2>>();
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
