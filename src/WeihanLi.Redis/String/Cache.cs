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

        bool Expire(string key, TimeSpan? expiresIn);

        Task<bool> ExpireAsync(string key, TimeSpan? expiresIn);

        #endregion Expire

        #region Exists

        bool Exists(string key);

        Task<bool> ExistsAsync(string key);

        #endregion Exists

        #region Get

        string Get(string key);

        Task<string> GetAsync(string key);

        T Get<T>(string key);

        Task<T> GetAsync<T>(string key);

        #endregion Get

        #region Set

        bool Set<T>(string key, T value);

        bool Set<T>(string key, T value, TimeSpan? expiresIn);

        bool Set<T>(string key, T value, TimeSpan? expiresIn, When when);

        bool Set<T>(string key, T value, TimeSpan? expiresIn, When when, CommandFlags commandFlags);

        Task<bool> SetAsync<T>(string key, T value);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiresIn, When when, CommandFlags commandFlags);

        #endregion Set

        #region Remove

        bool Remove(string key);

        Task<bool> RemoveAsync(string key);

        #endregion Remove
    }

    internal class CacheClient : BaseRedisClient, ICacheClient
    {
        public CacheClient() : base(LogHelper.GetLogHelper<CacheClient>(), new RedisWrapper("String/Cache/"))
        {
        }

        #region Exists

        public bool Exists(string key)
        {
            return Wrapper.Database.KeyExists(Wrapper.KeyPrefix + key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Wrapper.Database.KeyExistsAsync(Wrapper.KeyPrefix + key);
        }

        #endregion Exists

        #region Expire

        public bool Expire(string key, TimeSpan? expiresIn) => Wrapper.Database.KeyExpire(Wrapper.KeyPrefix + key, expiresIn);

        public Task<bool> ExpireAsync(string key, TimeSpan? expiresIn) => Wrapper.Database.KeyExpireAsync(Wrapper.KeyPrefix + key, expiresIn);

        #endregion Expire

        #region Get

        public T Get<T>(string key)
        {
            return Wrapper.Wrap<T>(Wrapper.KeyPrefix + key, (k) => Wrapper.Database.StringGet(k));
        }

        public string Get(string key)
        {
            return Wrapper.Wrap<string>(Wrapper.KeyPrefix + key, (k) => Wrapper.Database.StringGet(k));
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Wrapper.WrapAsync<T>(Wrapper.KeyPrefix + key, (k) => Wrapper.Database.StringGetAsync(k));
        }

        public Task<string> GetAsync(string key)
        {
            return Wrapper.WrapAsync<string>(Wrapper.KeyPrefix + key, (k) => Wrapper.Database.StringGetAsync(k));
        }

        #endregion Get

        public bool Remove(string key)
        {
            return Wrapper.Database.KeyDelete(Wrapper.KeyPrefix + key);
        }

        public Task<bool> RemoveAsync(string key)
        {
            return Wrapper.Database.KeyDeleteAsync(Wrapper.KeyPrefix + key);
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
