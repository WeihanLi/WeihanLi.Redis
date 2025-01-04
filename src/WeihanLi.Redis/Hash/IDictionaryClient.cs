using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface IDictionaryClient<TKey, TValue> : IRedisClient
    {
        #region Exists

        bool Exists(TKey fieldName, CommandFlags flags = CommandFlags.None);

        Task<bool> ExistsAsync(TKey fieldName, CommandFlags flags = CommandFlags.None);

        #endregion Exists

        #region Get

        TValue Get(TKey fieldName, CommandFlags flags = CommandFlags.None);

        Task<TValue> GetAsync(TKey fieldName, CommandFlags flags = CommandFlags.None);

        #endregion Get

        #region GetOrAdd

        TValue GetOrAdd(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None);

        Task<TValue> GetOrAddAsync(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None);

        TValue GetOrAdd(TKey fieldName, Func<TKey, TValue> func, CommandFlags flags = CommandFlags.None);

        Task<TValue> GetOrAddAsync(TKey fieldName, Func<TKey, Task<TValue>> func, CommandFlags flags = CommandFlags.None);

        #endregion GetOrAdd

        #region Set

        bool Add(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None);

        Task<bool> AddAsync(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None);

        bool Add(IDictionary<TKey, TValue> values, CommandFlags flags = CommandFlags.None);

        Task<bool> AddAsync(IDictionary<TKey, TValue> values, CommandFlags flags = CommandFlags.None);

        bool Set(TKey fieldName, TValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        Task<bool> SetAsync(TKey fieldName, TValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        #endregion Set

        #region Remove

        bool Remove(TKey fieldName, CommandFlags flags = CommandFlags.None);

        Task<bool> RemoveAsync(TKey fieldName, CommandFlags flags = CommandFlags.None);

        #endregion Remove

        #region Clear

        bool Clear(CommandFlags flags = CommandFlags.None);

        Task<bool> ClearAsync(CommandFlags flags = CommandFlags.None);

        #endregion Clear

        #region KeysValues

        /// <summary>Returns all field names in the hash stored at key.</summary>
        /// <returns>list of fields in the hash, or an empty list when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hkeys</remarks>
        TKey[] Keys(CommandFlags flags = CommandFlags.None);

        /// <summary>Returns all values in the hash stored at key.</summary>
        /// <returns>list of values in the hash, or an empty list when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hvals</remarks>
        TValue[] Values(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <returns>number of fields in the hash, or 0 when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hlen</remarks>
        long Count(CommandFlags flags = CommandFlags.None);

        /// <summary>Returns all field names in the hash stored at key.</summary>
        /// <returns>list of fields in the hash, or an empty list when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hkeys</remarks>
        Task<TKey[]> KeysAsync(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <returns>number of fields in the hash, or 0 when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hlen</remarks>
        Task<long> CountAsync(CommandFlags flags = CommandFlags.None);

        /// <summary>Returns all values in the hash stored at key.</summary>
        /// <returns>list of values in the hash, or an empty list when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/hvals</remarks>
        Task<TValue[]> ValuesAsync(CommandFlags flags = CommandFlags.None);

        #endregion KeysValues
    }
}
