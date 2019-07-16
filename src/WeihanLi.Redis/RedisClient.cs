using System;
using Microsoft.Extensions.Logging;

namespace WeihanLi.Redis
{
    public interface IRedisClient
    {
    }

    public abstract class BaseRedisClient
    {
        /// <summary>
        /// RandomGenerator
        /// </summary>
        protected readonly Random Random = new Random();

        public IRedisWrapper Wrapper { get; }

        /// <summary>
        /// logger
        /// </summary>
        protected ILogger Logger { get; }

        #region GetRandomCacheExpiry

        protected TimeSpan GetRandomCacheExpiry() => GetRandomCacheExpiry(RedisManager.RedisConfiguration.MaxRandomCacheExpiry);

        protected TimeSpan GetRandomCacheExpiry(int max) => TimeSpan.FromSeconds(Random.Next(max));

        protected TimeSpan GetRandomCacheExpiry(int min, int max) => TimeSpan.FromSeconds(Random.Next(min, max));

        #endregion GetRandomCacheExpiry

        protected BaseRedisClient(ILogger logger, IRedisWrapper redisWrapper)
        {
            Logger = logger;
            Wrapper = redisWrapper;
        }
    }
}
