using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    /// <summary>
    /// RateLimiter
    /// </summary>
    public interface IRateLimiterClient : IRedisClient
    {
        long Limit { get; }

        bool Acquire();

        Task<bool> AcquireAsync();

        bool Release();

        Task<bool> ReleaseAsync();
    }

    internal class RateLimiterClient : BaseRedisClient, IRateLimiterClient
    {
        private readonly string _limiterName;
        private readonly TimeSpan? _expiresIn;

        internal RateLimiterClient(string limiterName, long limit, TimeSpan? expiresIn, ILogger<RateLimiterClient> logger) : base(logger, new RedisWrapper(RedisDataType.RateLimiter))
        {
            _limiterName = Wrapper.GetRealKey(limiterName);
            _expiresIn = expiresIn;
            Limit = limit;
        }

        internal RateLimiterClient(string limiterName, TimeSpan? expiresIn, ILogger<RateLimiterClient> logger) : this(limiterName, 1, expiresIn, logger)
        {
        }

        public long Limit { get; }

        public bool Acquire()
        {
            if (_expiresIn.HasValue)
            {
                Wrapper.Database.StringSet(_limiterName, 0, _expiresIn, When.NotExists);
            }

            var result = Wrapper.Database.StringIncrement(_limiterName);
            if (result > Limit)
            {
                Wrapper.Database.StringDecrement(_limiterName);
                return false;
            }
            return true;
        }

        public async Task<bool> AcquireAsync()
        {
            if (_expiresIn.HasValue)
            {
                await Wrapper.Database.StringSetAsync(_limiterName, 0, _expiresIn, When.NotExists);
            }
            var result = await Wrapper.Database.StringIncrementAsync(_limiterName);
            if (result > Limit)
            {
                await Wrapper.Database.StringDecrementAsync(_limiterName).ConfigureAwait(false);
                return false;
            }
            return true;
        }

        public bool Release()
        {
            if (Wrapper.Database.KeyExists(_limiterName))
            {
                var result = Wrapper.Database.StringDecrement(_limiterName);
                if (result < 0)
                {
                    Wrapper.Database.StringIncrement(_limiterName);
                    return false;
                }
                return true;
            }

            if (_expiresIn.HasValue)
            {
                Wrapper.Database.StringSet(_limiterName, 0, _expiresIn, When.NotExists);
            }
            return false;
        }

        public async Task<bool> ReleaseAsync()
        {
            if (await Wrapper.Database.KeyExistsAsync(_limiterName))
            {
                var result = await Wrapper.Database.StringDecrementAsync(_limiterName);
                if (result < 0)
                {
                    await Wrapper.Database.StringIncrementAsync(_limiterName).ConfigureAwait(false);
                    return false;
                }
                return true;
            }
            if (_expiresIn.HasValue)
            {
                await Wrapper.Database.StringSetAsync(_limiterName, 0, _expiresIn, When.NotExists).ConfigureAwait(false);
            }
            return false;
        }
    }
}
