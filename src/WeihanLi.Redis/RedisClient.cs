using System;
using Microsoft.Extensions.Logging;

namespace WeihanLi.Redis
{
    public interface IRedisClient
    {
    }

    internal abstract class BaseRedisClient
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

        protected TimeSpan GetRandomCacheExpiry(int max)
        {
            return RedisManager.RedisConfiguration.EnableRandomExpiry 
                ? TimeSpan.FromSeconds(Random.Next(max))
                : TimeSpan.Zero
                ;
        }


        #endregion GetRandomCacheExpiry

        protected BaseRedisClient(ILogger logger, IRedisWrapper redisWrapper)
        {
            Logger = logger;
            Wrapper = redisWrapper;
        }
    }
}
