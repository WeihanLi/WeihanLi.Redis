// ReSharper disable once CheckNamespace
using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Redis
{
    public interface IHashClient
    {
        #region Expire

        bool Expire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None);

        Task<bool> ExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None);

        #endregion Expire

        #region Exists

        bool Exists(string key, string fieldName, CommandFlags flags = CommandFlags.None);

        Task<bool> ExistsAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None);

        #endregion Exists

        #region Get

        string Get(string key, string fieldName, CommandFlags flags = CommandFlags.None);

        Task<string> GetAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None);

        T Get<T>(string key, string fieldName, CommandFlags flags = CommandFlags.None);

        Task<T> GetAsync<T>(string key, string fieldName, CommandFlags flags = CommandFlags.None);

        #endregion Get

        #region Set

        bool Set<T>(string key, string fieldName, T value, When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> SetAsync<T>(string key, string fieldName, T value, When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

        #endregion Set

        #region Remove

        bool Remove(string key, string fieldName, CommandFlags flags = CommandFlags.None);

        Task<bool> RemoveAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None);

        #endregion Remove
    }

    /// <summary>
    /// https://github.com/antirez/redis/issues/167
    /// Hash does not support expire on field
    /// </summary>
    internal class HashClient : BaseRedisClient, IHashClient
    {
        public HashClient() : base(LogHelper.GetLogHelper<HashClient>(), new RedisWrapper("Hash/Cache/"))
        {
        }

        public bool Exists(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExists(Wrapper.KeyPrefix + key, fieldName, flags);

        public Task<bool> ExistsAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExistsAsync(Wrapper.KeyPrefix + key, fieldName, flags);

        public bool Expire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.KeyExpire(Wrapper.KeyPrefix + key, expiresIn, flags);
        }

        public Task<bool> ExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.KeyExpireAsync(Wrapper.KeyPrefix + key, expiresIn, flags);
        }

        public string Get(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Wrap<string>(() => Wrapper.Database.HashGet(Wrapper.KeyPrefix + key, fieldName, flags));

        public T Get<T>(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Wrap<T>(() => Wrapper.Database.HashGet(Wrapper.KeyPrefix + key, fieldName, flags));

        public Task<string> GetAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.WrapAsync<string>(() => Wrapper.Database.HashGetAsync(Wrapper.KeyPrefix + key, fieldName, flags));

        public Task<T> GetAsync<T>(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.WrapAsync<T>(() => Wrapper.Database.HashGetAsync(Wrapper.KeyPrefix + key, fieldName, flags));

        public bool Remove(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDelete(Wrapper.KeyPrefix + key, fieldName, flags);

        public Task<bool> RemoveAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDeleteAsync(Wrapper.KeyPrefix + key, fieldName, flags);

        public bool Set<T>(string key, string fieldName, T value) => Wrapper.Database.HashSet(Wrapper.KeyPrefix + key, fieldName, value.ToJsonOrString());

        public bool Set<T>(string key, string fieldName, T value, When when) => Wrapper.Database.HashSet(Wrapper.KeyPrefix + key, fieldName, value.ToJsonOrString(), when);

        public bool Set<T>(string key, string fieldName, T value, When when, CommandFlags commandFlags) => Wrapper.Database.HashSet(Wrapper.KeyPrefix + key, fieldName, value.ToJsonOrString(), when, commandFlags);

        public Task<bool> SetAsync<T>(string key, string fieldName, T value) => Wrapper.Database.HashSetAsync(Wrapper.KeyPrefix + key, fieldName, value.ToJsonOrString());

        public Task<bool> SetAsync<T>(string key, string fieldName, T value, When when) => Wrapper.Database.HashSetAsync(Wrapper.KeyPrefix + key, fieldName, value.ToJsonOrString(), when);

        public Task<bool> SetAsync<T>(string key, string fieldName, T value, When when, CommandFlags commandFlags) => Wrapper.Database.HashSetAsync(Wrapper.KeyPrefix + key, fieldName, value.ToJsonOrString(), when, commandFlags);
    }
}
