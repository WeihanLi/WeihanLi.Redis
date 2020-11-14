using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace WeihanLi.Redis.UnitTest
{
    public class Startup
    {
        private const int TestDbIndex = 7;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRedisConfig(config =>
            {
                config.RedisServers = new[]
                {
                    new RedisServerConfiguration("127.0.0.1", 6379),
                };
                config.CachePrefix = "WeihanLi.Redis.UnitTest";
                config.ClientName = "WeihanLi.Redis.UnitTest";

                config.DefaultDatabase = TestDbIndex;
            });
        }

        public void Config(IConnectionMultiplexer connectionMultiplexer)
        {
            var server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints(true)[0]);
            var db = connectionMultiplexer.GetDatabase(TestDbIndex);
            foreach (var key in server.Keys(TestDbIndex, "WeihanLi.Redis.UnitTest:*"))
            {
                db.KeyDelete(key);
            }
        }
    }
}
