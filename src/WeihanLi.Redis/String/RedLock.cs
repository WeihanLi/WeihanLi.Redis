using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface IRedLockClient : IDisposable, IRedisClient
    {
        /// <summary>
        /// TryGetLock
        /// </summary>
        /// <returns></returns>
        bool TryLock();

        /// <summary>
        /// TryGetLock
        /// </summary>
        /// <param name="expiry">lock expiry</param>
        /// <returns></returns>
        bool TryLock(TimeSpan? expiry);

        /// <summary>
        /// TryGetLockAsync
        /// </summary>
        /// <returns></returns>
        Task<bool> TryLockAsync();

        /// <summary>
        /// TryGetLockAsync
        /// </summary>
        /// <param name="expiry">lock expiry</param>
        /// <returns></returns>
        Task<bool> TryLockAsync(TimeSpan? expiry);

        /// <summary>
        /// ReleaseLock
        /// </summary>
        /// <returns></returns>
        bool Release();

        /// <summary>
        /// ReleaseLockAsync
        /// </summary>
        /// <returns></returns>
        Task<bool> ReleaseAsync();
    }

    internal class RedLockClient : BaseRedisClient, IRedLockClient
    {
        private readonly string _realKey;
        private readonly string _lockId;
        private bool _released;

        private readonly int _maxRetryCount;

        /// <summary>
        /// String containing the Lua unlock script.
        /// http://redis.cn/topics/distlock
        /// </summary>
        private const string UnlockScript = @"
            if redis.call(""get"",KEYS[1]) == ARGV[1] then
                return redis.call(""del"",KEYS[1])
            else
                return 0
            end";

        public RedLockClient(string key, ILogger<RedLockClient> logger) : this(key, 0, logger)
        {
        }

        public RedLockClient(string key, int maxRetryCount, ILogger<RedLockClient> logger) : base(logger, new RedisWrapper(RedisConstants.RedLockPrefix))
        {
            _realKey = Wrapper.GetRealKey(key);
            _lockId = $"{Environment.MachineName}__{Guid.NewGuid():N}";
            _maxRetryCount = maxRetryCount;
        }

        public bool Release()
        {
            _released = true;
            return (int)Wrapper.Database.ScriptEvaluate(UnlockScript,
                  new RedisKey[] { _realKey },
                  new[] { Wrapper.Wrap(_lockId) }) == 1;
        }

        public async Task<bool> ReleaseAsync()
        {
            _released = true;
            return (int)await Wrapper.Database.ScriptEvaluateAsync(UnlockScript,
                  new RedisKey[] { _realKey },
                  new[] { Wrapper.Wrap(_lockId) }) == 1;
        }

        public bool TryLock() => TryLock(null);

        public bool TryLock(TimeSpan? expiry)
        {
            var result = Wrapper.Database.StringSet(_realKey, Wrapper.Wrap(_lockId), expiry ?? TimeSpan.FromSeconds(RedisManager.RedisConfiguration.MaxLockExpiry), When.NotExists);

            if (!result && _maxRetryCount > 0)
            {
                result = RetryHelper.TryInvoke(
                    () =>
                    {
                        Thread.Sleep(RedisManager.RedisConfiguration.LockRetryDelay);
                        return Wrapper.Database.StringSet(_realKey, Wrapper.Wrap(_lockId),
                            expiry ?? TimeSpan.FromSeconds(RedisManager.RedisConfiguration.MaxLockExpiry),
                            When.NotExists);
                    }, r => r, _maxRetryCount);
            }

            return result;
        }

        public Task<bool> TryLockAsync() => TryLockAsync(null);

        public async Task<bool> TryLockAsync(TimeSpan? expiry)
        {
            var result = await Wrapper.Database.StringSetAsync(_realKey, Wrapper.Wrap(_lockId), expiry ?? TimeSpan.FromSeconds(RedisManager.RedisConfiguration.MaxLockExpiry), When.NotExists);

            if (!result && _maxRetryCount > 0)
            {
                result = await RetryHelper.TryInvokeAsync(
                    async () =>
                    {
                        await Task.Delay(RedisManager.RedisConfiguration.LockRetryDelay);
                        return await Wrapper.Database.StringSetAsync(_realKey, Wrapper.Wrap(_lockId),
                            expiry ?? TimeSpan.FromSeconds(RedisManager.RedisConfiguration.MaxLockExpiry),
                            When.NotExists);
                    }, r => r, _maxRetryCount);
            }

            return result;
        }

        #region IDisposable Support

        private bool _disposed; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (!_released)
                {
                    Release();
                }
                _disposed = true;
            }
        }

        // 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~RedLockClient()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
