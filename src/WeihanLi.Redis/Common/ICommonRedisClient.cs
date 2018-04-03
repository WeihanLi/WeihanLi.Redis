using System;
using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface ICommonRedisClient : IRedisClient
    {
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

        long KeyDelete(string[] keys, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None);

        Task<long> KeyDeleteAsync(string[] keys, CommandFlags flags = CommandFlags.None);

        #endregion KeyDelete

        #region TTL

        /// <summary>
        /// Returns the remaining time to live of a key that has a timeout.  This introspection capability allows a Redis client to check how many seconds a given key will continue to be part of the dataset.
        /// </summary>
        /// <returns>TTL, or nil when key does not exist or does not have a timeout.</returns>
        /// <remarks>http://redis.io/commands/ttl</remarks>
        TimeSpan? KeyTimeToLive(string key, CommandFlags flags = CommandFlags.None);

        #endregion TTL

        #region KeyType

        /// <summary>
        /// Returns the string representation of the type of the value stored at key. The different types that can be returned are: string, list, set, zset and hash.
        /// </summary>
        /// <returns>type of key, or none when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/type</remarks>
        RedisType KeyType(string key, CommandFlags flags = CommandFlags.None);

        #endregion KeyType

        #region Script

        RedisResult ScriptEvaluate<TValue>(string script, string[] keys = null, TValue[] values = null,
            CommandFlags flags = CommandFlags.None);

        Task<RedisResult> ScriptEvaluateAsync<TValue>(string script, string[] keys = null, TValue[] values = null,
            CommandFlags flags = CommandFlags.None);

        #endregion Script
    }
}
