using System;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;

namespace WeihanLi.Redis.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogHelper.ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
            });

            IServiceCollection services = new ServiceCollection();
            services.AddRedisConfig(options =>
            {
                options.RedisServers = new[]
                {
                    new RedisServerConfiguration("127.0.0.1"),
                    //new RedisServerConfiguration("127.0.0.1", 16379),
                };
            });
            // custom serializer
            //services.AddSingleton<IDataSerializer, BinaryDataSerializer>();
            DependencyResolver.SetDependencyResolver(services);

            //var cacheClient = DependencyResolver.Current.ResolveService<ICacheClient>();
            //var customSerializerCacheKey = "TestCustomSerializer";
            //cacheClient.Set(customSerializerCacheKey, new PagedListModel<int>()
            //{
            //    Data = new int[0],
            //    PageNumber = 2,
            //    PageSize = 10,
            //    TotalCount = 10,
            //}, TimeSpan.FromMinutes(1));
            //var result = cacheClient.Get<PagedListModel<int>>(customSerializerCacheKey);
            //Console.WriteLine(result.ToJson());

            var database = DependencyResolver.Current.GetRequiredService<IConnectionMultiplexer>()
                    .GetDatabase(0);
            database.HashSet("testHash", 1, 1);
            var hashVal = database.HashGet("testHash", 1);
            Console.WriteLine(hashVal);

            var updated = database.HashCompareAndExchange("testHash", 1, "2", "3");
            Console.WriteLine(updated);
            hashVal = database.HashGet("testHash", 1);
            Console.WriteLine(hashVal);

            updated = database.HashCompareAndExchange("testHash", 1, 4, 1);
            Console.WriteLine(updated);
            hashVal = database.HashGet("testHash", 1);
            Console.WriteLine(hashVal);

            database.StringSet("testString", 1);
            var stringVal = database.StringGet("testString");
            Console.WriteLine(stringVal);

            updated = database.StringCompareAndExchange("testString", 2, 3);
            Console.WriteLine(updated);
            stringVal = database.StringGet("testString");
            Console.WriteLine(stringVal);

            updated = database.StringCompareAndExchange("testString", 4, 1);
            Console.WriteLine(updated);
            stringVal = database.StringGet("testString");
            Console.WriteLine(stringVal);

            var c_name = "test_counter";
            database.StringSet(c_name, 0, TimeSpan.FromSeconds(10));

            var val = database.StringDecrement(c_name);
            Console.WriteLine(val);
            val = database.StringIncrement(c_name);
            Console.WriteLine(val);

            //try
            //{
            //    var cts = new CancellationTokenSource();
            //    var task = Task.Delay(3000, cts.Token);
            //    var task2 = Task.Delay(1000);

            //    cts.Cancel(true);

            //    Thread.Sleep(1000);

            //    Console.WriteLine($"task.IsCompleted:{task.IsCompleted}, task.IsCanceled:{task.IsCanceled}");
            //    Console.WriteLine($"task2.IsCompleted:{task2.IsCompleted}, task2.IsCanceled:{task2.IsCanceled}");
            //}
            //catch (TaskCanceledException ex)
            //{
            //    Console.WriteLine($"task canceled, ex:{ex}");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            //ConfigurationChangedEventSample.MainTest();

            Console.ReadLine();
        }
    }
}
