using System;
using System.Threading.Tasks;
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
        /// <param name="expiry">lock expiry</param>
        /// <returns></returns>
        bool TryLock(TimeSpan? expiry);

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
        private readonly Guid _lockId;

        public RedLockClient(string key) : base(LogHelper.GetLogHelper<RedLockClient>(), new RedisWrapper(RedisConstants.RedLockPrefix))
        {
            _realKey = Wrapper.GetRealKey(key);
            _lockId = Guid.NewGuid();
        }

        public bool Release()
        {
            if (Wrapper.Database.KeyExists(_realKey) && Wrapper.Unwrap<Guid>(Wrapper.Database.StringGet(_realKey)) == _lockId)
            {
                return Wrapper.Database.KeyDelete(_realKey);
            }
            return false;
        }

        public async Task<bool> ReleaseAsync()
        {
            if (await Wrapper.Database.KeyExistsAsync(_realKey) && Wrapper.Unwrap<Guid>(await Wrapper.Database.StringGetAsync(_realKey)) == _lockId)
            {
                return await Wrapper.Database.KeyDeleteAsync(_realKey);
            }
            return false;
        }

        public bool TryLock(TimeSpan? expiry)
        {
            return Wrapper.Database.StringSet(_realKey, Wrapper.Wrap(_lockId), expiry, When.NotExists);
        }

        public Task<bool> TryLockAsync(TimeSpan? expiry)
        {
            return Wrapper.Database.StringSetAsync(_realKey, Wrapper.Wrap(_lockId), expiry, When.NotExists);
        }

        #region IDisposable Support

        private bool _disposed; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Release();
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
