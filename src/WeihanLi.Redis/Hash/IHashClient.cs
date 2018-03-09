using System;
using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface IHashClient : IRedisClient
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

        #region KeysValues

        /// <summary>Returns all field names in the hash stored at key.</summary>
        /// <returns>list of fields in the hash, or an empty list when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hkeys</remarks>
        string[] Keys(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>Returns all values in the hash stored at key.</summary>
        /// <returns>list of values in the hash, or an empty list when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hvals</remarks>
        T[] Values<T>(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <returns>number of fields in the hash, or 0 when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hlen</remarks>
        long Length(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>Returns all field names in the hash stored at key.</summary>
        /// <returns>list of fields in the hash, or an empty list when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hkeys</remarks>
        Task<string[]> KeysAsync(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <returns>number of fields in the hash, or 0 when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hlen</remarks>
        Task<long> LengthAsync(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>Returns all values in the hash stored at key.</summary>
        /// <returns>list of values in the hash, or an empty list when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hvals</remarks>
        Task<T[]> ValuesAsync<T>(string key, CommandFlags flags = CommandFlags.None);

        #endregion KeysValues
    }
}
