using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    /// <summary>
    /// https://github.com/antirez/redis/issues/167
    /// Hash does not support expire on field
    /// </summary>
    internal class HashClient : BaseRedisClient, IHashClient
    {
        public HashClient() : base(LogHelper.GetLogHelper<HashClient>(), new RedisWrapper("Hash/Cache"))
        {
        }

        public bool Exists(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExists($"{Wrapper.KeyPrefix}/{key}", fieldName, flags);

        public Task<bool> ExistsAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExistsAsync($"{Wrapper.KeyPrefix}/{key}", fieldName, flags);

        public bool Expire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExpire(key, expiresIn, flags);

        public Task<bool> ExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExpireAsync(key, expiresIn, flags);

        public string Get(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<string>(() => Wrapper.Database.HashGet($"{Wrapper.KeyPrefix}/{key}", fieldName, flags));

        public T Get<T>(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.HashGet($"{Wrapper.KeyPrefix}/{key}", fieldName, flags));

        public Task<string> GetAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<string>(() => Wrapper.Database.HashGetAsync($"{Wrapper.KeyPrefix}/{key}", fieldName, flags));

        public Task<T> GetAsync<T>(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.HashGetAsync($"{Wrapper.KeyPrefix}/{key}", fieldName, flags));

        public string[] Keys(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashKeys(Wrapper.GetRealKey(key), flags).Select(_ => (string)_).ToArray();

        public Task<string[]> KeysAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashKeysAsync(Wrapper.GetRealKey(key), flags).ContinueWith(r => r.Result.Select(_ => (string)_).ToArray());

        public long Length(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashLength(Wrapper.GetRealKey(key), flags);

        public Task<long> LengthAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashLengthAsync(Wrapper.GetRealKey(key), flags);

        public bool Remove(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDelete($"{Wrapper.KeyPrefix}/{key}", fieldName, flags);

        public Task<bool> RemoveAsync(string key, string fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDeleteAsync($"{Wrapper.KeyPrefix}/{key}", fieldName, flags);

        public bool Set<T>(string key, string fieldName, T value, When when, CommandFlags commandFlags) => Wrapper.Database.HashSet($"{Wrapper.KeyPrefix}/{key}", fieldName, Wrapper.Wrap(value), when, commandFlags);

        public Task<bool> SetAsync<T>(string key, string fieldName, T value, When when, CommandFlags commandFlags) => Wrapper.Database.HashSetAsync($"{Wrapper.KeyPrefix}/{key}", fieldName, Wrapper.Wrap(value), when, commandFlags);

        public T[] Values<T>(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.HashValues(Wrapper.GetRealKey(key), flags));

        public async Task<T[]> ValuesAsync<T>(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.HashValuesAsync(Wrapper.GetRealKey(key), flags));
    }
}
