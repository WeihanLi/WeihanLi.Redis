using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    /// <summary>
    /// CacheClient
    /// </summary>
    public interface ICacheClient : IRedisClient
    {
        #region Expire

        bool Expire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None);

        Task<bool> ExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None);

        #endregion Expire

        #region Exists

        bool Exists(string key, CommandFlags flags = CommandFlags.None);

        Task<bool> ExistsAsync(string key, CommandFlags flags = CommandFlags.None);

        #endregion Exists

        #region Get

        string Get(string key, CommandFlags commandFlags = CommandFlags.None);

        Task<string> GetAsync(string key, CommandFlags commandFlags = CommandFlags.None);

        T Get<T>(string key, CommandFlags commandFlags = CommandFlags.None);

        Task<T> GetAsync<T>(string key, CommandFlags commandFlags = CommandFlags.None);

        #endregion Get

        #region Set

        bool Set<T>(string key, T value);

        bool Set<T>(string key, T value, TimeSpan? expiresIn, When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

        bool Set<T>(string key, Func<T> func, TimeSpan? expiresIn, When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> SetAsync<T>(string key, T value);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> SetAsync<T>(string key, Func<T> func, TimeSpan? expiresIn, When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> SetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiresIn, When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

        #endregion Set

        #region GetOrSet

        T GetOrSet<T>(string key, Func<T> func, TimeSpan? expiresIn = null, CommandFlags flags = CommandFlags.None);

        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiresIn = null, CommandFlags flags = CommandFlags.None);

        #endregion GetOrSet

        #region Remove

        bool Remove(string key, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> RemoveAsync(string key, CommandFlags commandFlags = CommandFlags.None);

        #endregion Remove
    }

    internal class CacheClient : BaseRedisClient, ICacheClient
    {
        public CacheClient(ILogger<CacheClient> logger) : base(logger, new RedisWrapper(RedisConstants.CachePrefix))
        {
        }

        #region Exists

        public bool Exists(string key, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExists(key, flags);

        public Task<bool> ExistsAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExistsAsync(key, flags);

        #endregion Exists

        #region Expire

        public bool Expire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExpire(key, expiresIn, flags);

        public Task<bool> ExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExpireAsync(key, expiresIn, flags);

        #endregion Expire

        #region Get

        public T Get<T>(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.Value.StringGet(Wrapper.GetRealKey(key), flags));

        public string Get(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<string>(() => Wrapper.Database.Value.StringGet(Wrapper.GetRealKey(key), flags));

        public Task<T> GetAsync<T>(string key, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.Value.StringGetAsync(Wrapper.GetRealKey(key), flags));

        public Task<string> GetAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<string>(() => Wrapper.Database.Value.StringGetAsync(Wrapper.GetRealKey(key), flags));

        #endregion Get

        public T GetOrSet<T>(string key, Func<T> func, TimeSpan? expiresIn = null, CommandFlags flags = CommandFlags.None)
        {
            if (!Exists(key, flags))
            {
                Set(key, func, expiresIn, When.NotExists, flags);
            }
            return Get<T>(key, flags);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiresIn = null, CommandFlags flags = CommandFlags.None)
        {
            if (!await ExistsAsync(key, flags))
            {
                await SetAsync(key, func, expiresIn, When.NotExists, flags);
            }
            return await GetAsync<T>(key, flags);
        }

        public bool Remove(string key, CommandFlags flags = CommandFlags.None) => Wrapper.KeyDelete(key, flags);

        public Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.KeyDeleteAsync(key, flags);

        public bool Set<T>(string key, T value) => Set(key, value, null);

        public bool Set<T>(string key, T value, TimeSpan? expiresIn) => Set(key, value, expiresIn, When.Always);

        public bool Set<T>(string key, T value, TimeSpan? expiresIn, When when) => Set(key, value, expiresIn, when, CommandFlags.None);

        public bool Set<T>(string key, T value, TimeSpan? expiresIn, When when, CommandFlags commandFlags) =>
            Set(key, () => value, expiresIn, when, commandFlags);

        public bool Set<T>(string key, Func<T> func, TimeSpan? expiresIn, When when, CommandFlags commandFlags)
        {
            var realKey = Wrapper.GetRealKey(key);
            using (var locker = new RedLockClient(HashHelper.GetHashedString(HashType.MD5, realKey), DependencyResolver.Current.ResolveService<ILogger<RedLockClient>>()))
            {
                if (locker.TryLock())
                {
                    return Wrapper.Database.Value.StringSet(realKey,
                        Wrapper.Wrap(func()),
                        (expiresIn ?? (RedisManager.RedisConfiguration.AllowNoExpiry ? null : (TimeSpan?)RedisManager.RedisConfiguration.MaxCacheExpiry))?.Add(GetRandomCacheExpiry()),
                        when,
                        commandFlags);
                }
                Logger.LogInformation($"get lock failed,update cache fail,cache key:{realKey},current time:{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}");
                return false;
            }
        }

        public Task<bool> SetAsync<T>(string key, T value) => SetAsync(key, value, null);

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn) => SetAsync(key, value, expiresIn, When.Always);

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when) => SetAsync(key, value, expiresIn, when, CommandFlags.None);

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when,
            CommandFlags commandFlags) => SetAsync(key, () => value, expiresIn, when, commandFlags);

        public async Task<bool> SetAsync<T>(string key, Func<T> func, TimeSpan? expiresIn, When when, CommandFlags commandFlags)
        {
            var realKey = Wrapper.GetRealKey(key);
            using (var locker = new RedLockClient(HashHelper.GetHashedString(HashType.MD5, realKey), DependencyResolver.Current.ResolveService<ILogger<RedLockClient>>()))
            {
                if (await locker.TryLockAsync())
                {
                    return await Wrapper.Database.Value.StringSetAsync(realKey,
                        Wrapper.Wrap(func()),
                        (expiresIn ?? (RedisManager.RedisConfiguration.AllowNoExpiry ? null : (TimeSpan?)RedisManager.RedisConfiguration.MaxCacheExpiry))?.Add(GetRandomCacheExpiry()),
                        when,
                        commandFlags);
                }
                Logger.LogInformation($"get lock failed,update cache fail,cache key:{realKey},current time:{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}");
                return false;
            }
        }

        public async Task<bool> SetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiresIn, When when, CommandFlags commandFlags)
        {
            var realKey = Wrapper.GetRealKey(key);
            using (var locker = new RedLockClient(HashHelper.GetHashedString(HashType.MD5, realKey), DependencyResolver.Current.ResolveService<ILogger<RedLockClient>>()))
            {
                if (await locker.TryLockAsync())
                {
                    return await Wrapper.Database.Value.StringSetAsync(realKey, Wrapper.Wrap(await func()), expiresIn?.Add(GetRandomCacheExpiry()), when,
                        commandFlags);
                }
                Logger.LogInformation($"get lock failed,update cache fail,cache key:{realKey},current time:{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}");
                return false;
            }
        }
    }
}
