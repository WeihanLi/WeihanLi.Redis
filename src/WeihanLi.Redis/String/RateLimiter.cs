using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

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

            return Wrapper.Database.StringIncrement(_limiterName) <= Limit;
        }

        public async Task<bool> AcquireAsync()
        {
            if (_expiresIn.HasValue)
            {
                await Wrapper.Database.StringSetAsync(_limiterName, 0, _expiresIn, When.NotExists);
            }
            return await Wrapper.Database.StringIncrementAsync(_limiterName).ContinueWith(r => r.Result <= Limit);
        }

        public bool Release()
        {
            if (Wrapper.Database.KeyExists(_limiterName))
            {
                return Wrapper.Database.StringDecrement(_limiterName) >= 0;
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
                return await Wrapper.Database.StringDecrementAsync(_limiterName).ContinueWith(r => r.Result > 0);
            }
            if (_expiresIn.HasValue)
            {
                await Wrapper.Database.StringSetAsync(_limiterName, 0, _expiresIn, When.NotExists).ConfigureAwait(false);
            }
            return false;
        }
    }
}
