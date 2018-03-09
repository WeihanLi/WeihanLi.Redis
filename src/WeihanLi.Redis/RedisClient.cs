using System.Linq;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;
using WeihanLi.Redis.Internals;

namespace WeihanLi.Redis
{
    public interface IRedisClient
    {
    }

    internal abstract class BaseRedisClient
    {
        private static readonly ConnectionMultiplexer Connection;

        public IRedisWrapper Wrapper { get; }

        /// <summary>
        /// logger
        /// </summary>
        protected LogHelper Logger { get; }

        static BaseRedisClient()
        {
            var configurationOptions = new ConfigurationOptions()
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

            Connection = ConnectionMultiplexer.Connect(configurationOptions);
        }

        protected BaseRedisClient(LogHelper logger, IRedisWrapper redisWrapper)
        {
            Logger = logger;
            Wrapper = redisWrapper;
            Wrapper.Database = Connection.GetDatabase();
            Wrapper.Subscriber = Connection.GetSubscriber();
        }
    }
}
