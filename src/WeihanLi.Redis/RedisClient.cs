using StackExchange.Redis;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    internal interface IRedisClient
    {
        IRedisWrapper Wrapper { get; }
    }

    internal abstract class BaseRedisClient : IRedisClient
    {
        private static readonly string redisConn = $"{RedisManager.RedisConfiguration.Host}:{RedisManager.RedisConfiguration.Port},Password={RedisManager.RedisConfiguration.Password},DefaultDatabase={RedisManager.RedisConfiguration.Database},ssl={RedisManager.RedisConfiguration.Ssl}";

        private static ConnectionMultiplexer connection;

        public IRedisWrapper Wrapper { get; }

        /// <summary>
        /// logger
        /// </summary>
        protected LogHelper Logger { get; }

        static BaseRedisClient()
        {
            connection = ConnectionMultiplexer.Connect(redisConn);
        }

        public BaseRedisClient(LogHelper logger, IRedisWrapper redisWrapper)
        {
            Logger = logger;
            Wrapper = redisWrapper;
            Wrapper.Database = connection.GetDatabase();
        }
    }
}
