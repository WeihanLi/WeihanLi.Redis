using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    /// <summary>
    /// https://github.com/antirez/redis/issues/167
    /// Hash does not support expire on field
    /// </summary>
    internal class HashClient : BaseRedisClient, IHashClient
    {
        public HashClient(ILogger<HashClient> logger) : base(logger, new RedisWrapper(RedisConstants.HashPrefix))
        {
        }

        public bool Exists(string key, RedisValue fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExists(Wrapper.GetRealKey(key), fieldName, flags);

        public Task<bool> ExistsAsync(string key, RedisValue fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExistsAsync(Wrapper.GetRealKey(key), fieldName, flags);

        public bool Expire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExpire(key, expiresIn, flags);

        public Task<bool> ExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Wrapper.KeyExpireAsync(key, expiresIn, flags);

        public string Get(string key, RedisValue fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<string>(() => Wrapper.Database.HashGet(Wrapper.GetRealKey(key), fieldName, flags));

        public T Get<T>(string key, RedisValue fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.HashGet(Wrapper.GetRealKey(key), fieldName, flags));

        public Task<string> GetAsync(string key, RedisValue fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<string>(() => Wrapper.Database.HashGetAsync(Wrapper.GetRealKey(key), fieldName, flags));

        public Task<T> GetAsync<T>(string key, RedisValue fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.HashGetAsync(Wrapper.GetRealKey(key), fieldName, flags));

        public string[] Get(string key, RedisValue[] fieldNames, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<string>(() => Wrapper.Database.HashGet(Wrapper.GetRealKey(key), fieldNames, flags));

        public T[] Get<T>(string key, RedisValue[] fieldNames, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.HashGet(Wrapper.GetRealKey(key), fieldNames, flags));

        public Task<string[]> GetAsync(string key, RedisValue[] fieldNames, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<string>(() => Wrapper.Database.HashGetAsync(Wrapper.GetRealKey(key), fieldNames, flags));

        public Task<T[]> GetAsync<T>(string key, RedisValue[] fieldNames, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.HashGetAsync(Wrapper.GetRealKey(key), fieldNames, flags));

        public T GetOrSet<T>(string key, RedisValue fieldName, Func<T> func, CommandFlags flags = CommandFlags.None)
        {
            if (Exists(key, fieldName, flags))
            {
                return Get<T>(key, fieldName, flags);
            }
            var val = func();
            var result = Set(key, fieldName, val, When.NotExists, flags);
            return result ? val : Get<T>(key, fieldName, flags);
        }

        public async Task<T> GetOrSetAsync<T>(string key, RedisValue fieldName, Func<Task<T>> func, CommandFlags flags = CommandFlags.None)
        {
            if (await ExistsAsync(key, fieldName, flags))
            {
                return await GetAsync<T>(key, fieldName, flags);
            }
            var value = await func();
            var result = await SetAsync(key, fieldName, value, When.NotExists, flags);
            return result ? value : await GetAsync<T>(key, fieldName, flags);
        }

        public string[] Keys(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashKeys(Wrapper.GetRealKey(key), flags).Select(_ => (string)_).ToArray();

        public Task<string[]> KeysAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashKeysAsync(Wrapper.GetRealKey(key), flags).ContinueWith(r => r.Result.Select(_ => (string)_).ToArray());

        public long Length(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashLength(Wrapper.GetRealKey(key), flags);

        public Task<long> LengthAsync(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashLengthAsync(Wrapper.GetRealKey(key), flags);

        public bool Remove(string key, RedisValue fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDelete(Wrapper.GetRealKey(key), fieldName, flags);

        public Task<bool> RemoveAsync(string key, RedisValue fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDeleteAsync(Wrapper.GetRealKey(key), fieldName, flags);

        public bool Set<T>(string key, RedisValue fieldName, T value, When when, CommandFlags commandFlags) => Wrapper.Database.HashSet(Wrapper.GetRealKey(key), fieldName, Wrapper.Wrap(value), when, commandFlags);

        public bool Set<T>(string key, IEnumerable<KeyValuePair<string, T>> entries, CommandFlags commandFlags = CommandFlags.None)
        {
            Wrapper.Database.HashSet(Wrapper.GetRealKey(key), entries.Select(_ => new HashEntry(_.Key, Wrapper.Wrap(_.Value))).ToArray(), commandFlags);
            return true;
        }

        public Task<bool> SetAsync<T>(string key, RedisValue fieldName, T value, When when, CommandFlags commandFlags) => Wrapper.Database.HashSetAsync(Wrapper.GetRealKey(key), fieldName, Wrapper.Wrap(value), when, commandFlags);

        public Task<bool> SetAsync<T>(string key, IEnumerable<KeyValuePair<string, T>> entries,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Wrapper.Database.HashSetAsync(Wrapper.GetRealKey(key), entries.Select(_ => new HashEntry(_.Key, Wrapper.Wrap(_.Value))).ToArray(), commandFlags).ContinueWith(r => true);
        }

        public T[] Values<T>(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.HashValues(Wrapper.GetRealKey(key), flags));

        public async Task<T[]> ValuesAsync<T>(string key, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.HashValuesAsync(Wrapper.GetRealKey(key), flags));
    }
}
