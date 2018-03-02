using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    /// <summary>
    /// CacheClient
    /// </summary>
    public interface ICacheClient
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

        Task<bool> SetAsync<T>(string key, T value);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

        #endregion Set

        #region GetOrSet

        T GetOrSet<T>(string key, Func<T> func, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        #endregion GetOrSet

        #region Remove

        bool Remove(string key, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> RemoveAsync(string key, CommandFlags commandFlags = CommandFlags.None);

        #endregion Remove
    }

    internal class CacheClient : BaseRedisClient, ICacheClient
    {
        private readonly string _prefix;

        public CacheClient() : this(null)
        {
        }

        public CacheClient(string prefix) : base(LogHelper.GetLogHelper<CacheClient>(), new RedisWrapper("String/Cache"))
        {
            _prefix = string.IsNullOrWhiteSpace(prefix) ? "Default" : prefix;
        }

        #region Exists

        public bool Exists(string key, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExists($"{_prefix}/{key}", flags);

        public Task<bool> ExistsAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExistsAsync($"{_prefix}/{key}", flags);

        #endregion Exists

        #region Expire

        public bool Expire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExpire($"{_prefix}/{key}", expiresIn, flags);

        public Task<bool> ExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExpireAsync($"{_prefix}/{key}", expiresIn, flags);

        #endregion Expire

        #region Get

        public T Get<T>(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.StringGet($"{Wrapper.KeyPrefix}/{_prefix}/{key}", flags));

        public string Get(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<string>(() => Wrapper.Database.StringGet($"{Wrapper.KeyPrefix}/{_prefix}/{key}", flags));

        public Task<T> GetAsync<T>(string key, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.StringGetAsync($"{Wrapper.KeyPrefix}/{_prefix}/{key}", flags));

        public Task<string> GetAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<string>(() => Wrapper.Database.StringGetAsync($"{Wrapper.KeyPrefix}/{_prefix}/{key}", flags));

        #endregion Get

        public T GetOrSet<T>(string key, Func<T> func, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (Exists(key, flags))
            {
                return Get<T>(key, flags);
            }
            var val = func();
            Set(key, val, expiresIn, When.NotExists, flags);
            return val;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (await ExistsAsync(key, flags))
            {
                return await GetAsync<T>(key, flags);
            }
            var val = await func();
            await SetAsync(key, val, expiresIn, When.NotExists, flags);
            return val;
        }

        public bool Remove(string key, CommandFlags flags = CommandFlags.None) => Wrapper.KeyDelete($"{_prefix}/{key}", flags);

        public Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.KeyDeleteAsync($"{_prefix}/{key}", flags);

        public bool Set<T>(string key, T value) => Set(key, value, null);

        public bool Set<T>(string key, T value, TimeSpan? expiresIn) => Set(key, value, expiresIn, When.Always);

        public bool Set<T>(string key, T value, TimeSpan? expiresIn, When when) => Set(key, value, expiresIn, when, CommandFlags.None);

        public bool Set<T>(string key, T value, TimeSpan? expiresIn, When when, CommandFlags commandFlags) => Wrapper.Database.StringSet($"{Wrapper.KeyPrefix}/{_prefix}/{key}", Wrapper.Wrap(value), expiresIn, when, commandFlags);

        public Task<bool> SetAsync<T>(string key, T value) => SetAsync(key, value, null);

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn) => SetAsync(key, value, expiresIn, When.Always);

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when) => SetAsync(key, value, expiresIn, when, CommandFlags.None);

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when, CommandFlags commandFlags) => Wrapper.Database.StringSetAsync($"{Wrapper.KeyPrefix}/{_prefix}/{key}", Wrapper.Wrap(value), expiresIn, when, commandFlags);
    }
}
