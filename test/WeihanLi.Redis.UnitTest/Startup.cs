using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace WeihanLi.Redis.UnitTest
{
    public class Startup
    {
        private const int TestDbIndex = 0;
        private const string DefaultRedisHost = "127.0.0.1";

        public void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureHostConfiguration(builder =>
              builder.AddEnvironmentVariables("App")
            );
        }

        public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
        {
            services.AddRedisConfig(config =>
            {
                config.RedisServers = new[]
                {
                    new RedisServerConfiguration(context.Configuration.GetConnectionString("Redis") ?? DefaultRedisHost, 6379),
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
