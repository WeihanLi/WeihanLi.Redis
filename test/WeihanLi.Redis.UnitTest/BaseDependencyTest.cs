using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis.UnitTest
{
    public class BaseDependencyTest
    {
        static BaseDependencyTest()
        {
            LogHelper.LogInit();
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
