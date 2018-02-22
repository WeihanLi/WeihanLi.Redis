// ReSharper disable once CheckNamespace
using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

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
        public CacheClient() : base(LogHelper.GetLogHelper<CacheClient>(), new RedisWrapper("String/Cache/"))
        {
        }

        #region Exists

        public bool Exists(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.KeyExists(Wrapper.KeyPrefix + key, flags);

        public Task<bool> ExistsAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.KeyExistsAsync(Wrapper.KeyPrefix + key, flags);

        #endregion Exists

        #region Expire

        public bool Expire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.Database.KeyExpire(Wrapper.KeyPrefix + key, expiresIn, flags);

        public Task<bool> ExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.Database.KeyExpireAsync(Wrapper.KeyPrefix + key, expiresIn, flags);

        #endregion Expire

        #region Get

        public T Get<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Wrap<T>(Wrapper.KeyPrefix + key, (k) => Wrapper.Database.StringGet(k, flags));
        }

        public string Get(string key, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Wrap<string>(Wrapper.KeyPrefix + key, (k) => Wrapper.Database.StringGet(k, flags));
        }

        public Task<T> GetAsync<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.WrapAsync<T>(Wrapper.KeyPrefix + key, (k) => Wrapper.Database.StringGetAsync(k, flags));
        }

        public Task<string> GetAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.WrapAsync<string>(Wrapper.KeyPrefix + key, (k) => Wrapper.Database.StringGetAsync(k, flags));
        }

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

        public bool Remove(string key, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.KeyDelete(Wrapper.KeyPrefix + key, flags);
        }

        public Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.KeyDeleteAsync(Wrapper.KeyPrefix + key, flags);
        }

        public bool Set<T>(string key, T value)
        {
            return Set(key, value, null);
        }

        public bool Set<T>(string key, T value, TimeSpan? expiresIn)
        {
            return Set(key, value, expiresIn, When.Always);
        }

        public bool Set<T>(string key, T value, TimeSpan? expiresIn, When when)
        {
            return Set(key, value, expiresIn, when, CommandFlags.None);
        }

        public bool Set<T>(string key, T value, TimeSpan? expiresIn, When when, CommandFlags commandFlags)
        {
            return Wrapper.Database.StringSet(Wrapper.KeyPrefix + key, value.ToJsonOrString(), expiresIn, when, commandFlags);
        }

        public Task<bool> SetAsync<T>(string key, T value)
        {
            return SetAsync(key, value, null);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn)
        {
            return SetAsync(key, value, expiresIn, When.Always);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when)
        {
            return SetAsync(key, value, expiresIn, when, CommandFlags.None);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when, CommandFlags commandFlags)
        {
            return Wrapper.Database.StringSetAsync(Wrapper.KeyPrefix + key, value.ToJsonOrString(), expiresIn, when, commandFlags);
        }
    }
}
