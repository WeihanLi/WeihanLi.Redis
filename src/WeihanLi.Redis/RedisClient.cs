using System.Linq;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Redis
{
    internal interface IRedisClient
    {
        IRedisWrapper Wrapper { get; }
    }

    internal abstract class BaseRedisClient : IRedisClient
    {
        private static readonly ConnectionMultiplexer Connection;

        public IRedisWrapper Wrapper { get; }

        /// <summary>
        /// logger
        /// </summary>
        protected LogHelper Logger { get; }

        static BaseRedisClient()
        {
            // TODO:ValidateRedisConfig
            Connection = ConnectionMultiplexer.Connect($"{string.Join(",", RedisManager.RedisConfiguration.RedisServers.Select(_ => $"{_.Host}:{_.Port}"))},password={RedisManager.RedisConfiguration.Password},defaultDatabase={RedisManager.RedisConfiguration.DefaultDatabase},ssl={RedisManager.RedisConfiguration.Ssl},connectRetry={RedisManager.RedisConfiguration.ConnectRetry},connectTimeout={RedisManager.RedisConfiguration.ConnectTimeout},proxy={RedisManager.RedisConfiguration.Proxy}");
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
