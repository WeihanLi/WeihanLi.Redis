using System;
using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
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
}
