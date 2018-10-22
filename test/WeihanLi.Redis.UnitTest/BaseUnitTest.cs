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
                config.CachePrefix = "WeihanLi.Redis.UnitTest";
                config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
                config.EnableCompress = false;
            });
            DependencyResolver.SetDependencyResolver(serviceCollection.BuildServiceProvider());
        }
    }
}
