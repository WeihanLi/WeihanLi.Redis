using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;

namespace WeihanLi.Redis.UnitTest
{
    public class BaseUnitTest
    {
        static BaseUnitTest()
        {
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
            });
            DependencyResolver.SetDependencyResolver(serviceCollection);
        }
    }
}
