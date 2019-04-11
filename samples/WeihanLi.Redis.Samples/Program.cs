using System;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Redis.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddRedisConfig();
            // custom serializer
            services.AddSingleton<IDataSerializer, BinaryDataSerializer>();
            DependencyResolver.SetDependencyResolver(services);

            var cacheClient = DependencyResolver.Current.ResolveService<ICacheClient>();
            var customSerializerCacheKey = "TestCustomSerializer";
            cacheClient.Set(customSerializerCacheKey, new PagedListModel<int>()
            {
                Data = new int[0],
                PageNumber = 2,
                PageSize = 10,
                TotalCount = 10,
            }, TimeSpan.FromMinutes(1));
            var result = cacheClient.Get<PagedListModel<int>>(customSerializerCacheKey);
            Console.WriteLine(result.ToJson());

            Console.ReadLine();
        }
    }
}
