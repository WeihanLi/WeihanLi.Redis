using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Extensions;
using WeihanLi.Redis.Internals;

namespace WeihanLi.Redis
{
    public interface IRedisClient
    {
    }

    internal abstract class BaseRedisClient
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;

        /// <summary>
        /// 随机数生成器
        /// </summary>
        protected readonly Random Random = new Random();

        public IRedisWrapper Wrapper { get; }

        /// <summary>
        /// logger
        /// </summary>
        protected ILogger Logger { get; }

        static BaseRedisClient()
        {
            var configurationOptions = new ConfigurationOptions
            {
                Password = RedisManager.RedisConfiguration.Password,
                DefaultDatabase = RedisManager.RedisConfiguration.DefaultDatabase,
                ConnectRetry = RedisManager.RedisConfiguration.ConnectRetry,
                ConnectTimeout = RedisManager.RedisConfiguration.ConnectTimeout,
                AllowAdmin = RedisManager.RedisConfiguration.AllowAdmin,
                Ssl = RedisManager.RedisConfiguration.Ssl,
                Proxy = RedisManager.RedisConfiguration.Proxy,
                AbortOnConnectFail = RedisManager.RedisConfiguration.AbortOnConnectFail,
                SyncTimeout = RedisManager.RedisConfiguration.SyncTimeout
            };
            configurationOptions.EndPoints.AddRange(RedisManager.RedisConfiguration.RedisServers.Select(s => Helpers.ConvertToEndPoint(s.Host, s.Port)).ToArray());
            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions));
        }

        #region GetRandomCacheExpiry

        protected TimeSpan GetRandomCacheExpiry() => GetRandomCacheExpiry(RedisManager.RedisConfiguration.MaxRandomCacheExpiry);

        protected TimeSpan GetRandomCacheExpiry(int max) => TimeSpan.FromSeconds(Random.Next(max));

        protected TimeSpan GetRandomCacheExpiry(int min, int max) => TimeSpan.FromSeconds(Random.Next(min, max));

        #endregion GetRandomCacheExpiry

        protected BaseRedisClient(ILogger logger, IRedisWrapper redisWrapper)
        {
            Logger = logger;
            Wrapper = redisWrapper;
            Wrapper.Database = Connection.Value.GetDatabase();
            Wrapper.Subscriber = Connection.Value.GetSubscriber();
        }
    }
}
