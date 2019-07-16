using System;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging.Log4Net;

namespace WeihanLi.Redis.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogHelper.AddLogProvider(new Log4NetLogHelperProvider());

            IServiceCollection services = new ServiceCollection();
            services.AddRedisConfig(options =>
            {
                options.EnableCompress = true;
                options.RedisServers = new[]
                {
                    //new RedisServerConfiguration("127.0.0.1"),
                    new RedisServerConfiguration("127.0.0.1", 16379),
                };
            });
            // custom serializer
            services.AddSingleton<IDataSerializer, BinaryDataSerializer>();
            services.AddSingleton<IDataCompressor, NullDataCompressor>();
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

            ConfigurationChangedEventSample.MainTest();

            Console.ReadLine();
        }
    }
}
