using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Redis
{
    internal interface IRedisWrapper
    {
        /// <summary>
        /// DataSerializer
        /// </summary>
        IDataSerializer DataSerializer { get; }

        /// <summary>
        /// Database
        /// </summary>
        Lazy<IDatabase> Database { get; set; }

        /// <summary>
        /// Subscriber
        /// </summary>
        Lazy<ISubscriber> Subscriber { get; set; }

        /// <summary>
        /// KeyPrefix
        /// </summary>
        string KeyPrefix { get; }

        string GetRealKey(string key);

        #region KeyExists

        bool KeyExists(string key, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyExistsAsync(string key, CommandFlags flags = CommandFlags.None);

        #endregion KeyExists

        #region KeyExpire

        bool KeyExpire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None);

        bool KeyExpire(string key, DateTime? expiry, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyExpireAsync(string key, DateTime? expiry, CommandFlags flags = CommandFlags.None);

        #endregion KeyExpire

        #region KeyPersist

        bool KeyPersist(string key, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyPersistAsync(string key, CommandFlags flags = CommandFlags.None);

        #endregion KeyPersist

        #region KeyDelete

        bool KeyDelete(string key, CommandFlags flags = CommandFlags.None);

        long KeyDelete(string[] key, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None);

        Task<long> KeyDeleteAsync(string[] key, CommandFlags flags = CommandFlags.None);

        #endregion KeyDelete

        /// <summary>
        /// Returns the remaining time to live of a key that has a timeout.  This introspection capability allows a Redis client to check how many seconds a given key will continue to be part of the dataset.
        /// </summary>
        /// <returns>TTL, or nil when key does not exist or does not have a timeout.</returns>
        /// <remarks>http://redis.io/commands/ttl</remarks>
        TimeSpan? KeyTimeToLive(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the string representation of the type of the value stored at key. The different types that can be returned are: string, list, set, zset and hash.
        /// </summary>
        /// <returns>type of key, or none when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/type</remarks>
        RedisType KeyType(string key, CommandFlags flags = CommandFlags.None);

        #region Script

        RedisResult ScriptEvaluate<TValue>(string script, string[] keys = null, TValue[] values = null,
            CommandFlags flags = CommandFlags.None);

        Task<RedisResult> ScriptEvaluateAsync<TValue>(string script, string[] keys = null, TValue[] values = null,
            CommandFlags flags = CommandFlags.None);

        #endregion Script

        #region Wrap

        RedisValue Wrap<T>(T t);

        RedisValue[] Wrap<T>(T[] ts);

        RedisValue Wrap<T>(Func<T> func);

        Task<RedisValue> WrapAsync<T>(Func<Task<T>> func);

        #endregion Wrap

        #region Unwrap

        T Unwrap<T>(RedisValue redisValue);

        T Unwrap<T>(Func<RedisValue> func);

        Task<T> UnwrapAsync<T>(Func<Task<RedisValue>> func);

        T[] Unwrap<T>(RedisValue[] values);

        T[] Unwrap<T>(Func<RedisValue[]> func);

        Task<T[]> UnwrapAsync<T>(Func<Task<RedisValue[]>> func);

        #endregion Unwrap
    }

    internal class RedisWrapper : IRedisWrapper
    {
        public IDataSerializer DataSerializer { get; }

        public Lazy<IDatabase> Database { get; set; }

        public string KeyPrefix { get; }

        public Lazy<ISubscriber> Subscriber { get; set; }

        public RedisWrapper(string keyPrefix)
        {
            KeyPrefix = $"{RedisManager.RedisConfiguration.CachePrefix}{RedisManager.RedisConfiguration.KeySeparator}{keyPrefix}";
            DataSerializer = RedisManager.RedisConfiguration.EnableCompress ?
                DependencyResolver.Current.ResolveService<CompressDataSerializer>()
                : DependencyResolver.Current.ResolveService<IDataSerializer>();
        }

        public RedisValue Wrap<T>(T t)
        {
            if (t == null)
            {
                return RedisValue.Null;
            }
            var type = typeof(T);
            if (type.IsBasicType())
            {
                return t.ToString();
            }
            else
            {
                return DataSerializer.Serialize(t);
            }
        }

        public RedisValue[] Wrap<T>(T[] ts) => ts.Select(Wrap).ToArray();

        public RedisValue Wrap<T>(Func<T> func) => Wrap(func());

        public async Task<RedisValue> WrapAsync<T>(Func<Task<T>> func) => Wrap(await func());

        public T Unwrap<T>(RedisValue redisValue)
        {
            if (redisValue.IsNull)
            {
                return default(T);
            }
            var type = typeof(T);
            if (type.IsBasicType())
            {
                return ((string)redisValue).To<T>();
            }
            else
            {
                return DataSerializer.Deserialize<T>(redisValue);
            }
        }

        public T Unwrap<T>(Func<RedisValue> func) => Unwrap<T>(func());

        public async Task<T> UnwrapAsync<T>(Func<Task<RedisValue>> func) => Unwrap<T>(await func());

        public T[] Unwrap<T>(RedisValue[] values) => values.Select(Unwrap<T>).ToArray();

        public T[] Unwrap<T>(Func<RedisValue[]> func) => Unwrap<T>(func());

        public async Task<T[]> UnwrapAsync<T>(Func<Task<RedisValue[]>> func) => Unwrap<T>(await func());

        public string GetRealKey(string key) => $"{KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{key}";

        public bool KeyExists(string key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyExists(GetRealKey(key), flags);

        public Task<bool> KeyExistsAsync(string key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyExistsAsync(GetRealKey(key), flags);

        public bool KeyExpire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Database.Value.KeyExpire(GetRealKey(key), expiresIn, flags);

        public Task<bool> KeyExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Database.Value.KeyExpireAsync(GetRealKey(key), expiresIn, flags);

        public bool KeyExpire(string key, DateTime? expiry, CommandFlags flags = CommandFlags.None) => Database.Value.KeyExpire(GetRealKey(key), expiry, flags);

        public Task<bool> KeyExpireAsync(string key, DateTime? expiry, CommandFlags flags = CommandFlags.None) => Database.Value.KeyExpireAsync(GetRealKey(key), expiry, flags);

        public bool KeyPersist(string key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyPersist(GetRealKey(key), flags);

        public Task<bool> KeyPersistAsync(string key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyPersistAsync(GetRealKey(key), flags);

        public bool KeyDelete(string key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyDelete(GetRealKey(key), flags);

        public Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyDeleteAsync(GetRealKey(key), flags);

        public long KeyDelete(string[] key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyDelete(key.Select(_ => (RedisKey)GetRealKey(_)).ToArray(), flags);

        public Task<long> KeyDeleteAsync(string[] key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyDeleteAsync(key.Select(_ => (RedisKey)GetRealKey(_)).ToArray(), flags);

        public TimeSpan? KeyTimeToLive(string key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyTimeToLive(GetRealKey(key), flags);

        public RedisType KeyType(string key, CommandFlags flags = CommandFlags.None) => Database.Value.KeyType(GetRealKey(key), flags);

        public RedisResult ScriptEvaluate<TValue>(string script, string[] keys = null, TValue[] values = null, CommandFlags flags = CommandFlags.None) => Database.Value.ScriptEvaluate(script, keys?.Select(_ => (RedisKey)GetRealKey(_)).ToArray(),
            null == values ? null : Wrap(values), flags);

        public Task<RedisResult> ScriptEvaluateAsync<TValue>(string script, string[] keys = null, TValue[] values = null, CommandFlags flags = CommandFlags.None) => Database.Value.ScriptEvaluateAsync(script, keys?.Select(_ => (RedisKey)GetRealKey(_)).ToArray(),
            null == values ? null : Wrap(values), flags);
    }
}
