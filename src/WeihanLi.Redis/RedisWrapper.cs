using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
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
        IDatabase Database { get; set; }

        /// <summary>
        /// Subscriber
        /// </summary>
        ISubscriber Subscriber { get; set; }

        /// <summary>
        /// KeyPrefix
        /// </summary>
        string KeyPrefix { get; }

        /// <summary>
        /// ChannelPrefix
        /// </summary>
        string ChannelPrefix { get; }

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

        #region KeyDelete

        bool KeyDelete(string key, CommandFlags flags = CommandFlags.None);

        long KeyDelete(string[] key, CommandFlags flags = CommandFlags.None);

        Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None);

        Task<long> KeyDeleteAsync(string[] key, CommandFlags flags = CommandFlags.None);

        #endregion KeyDelete

        RedisValue Wrap<T>(T t);

        RedisValue Wrap<T>(Func<T> func);

        Task<RedisValue> WrapAsync<T>(Func<Task<T>> func);

        T Unwrap<T>(RedisValue redisValue);

        T Unwrap<T>(Func<RedisValue> func);

        Task<T> UnwrapAsync<T>(Func<Task<RedisValue>> func);

        T[] Unwrap<T>(Func<RedisValue[]> func);

        Task<T[]> UnwrapAsync<T>(Func<Task<RedisValue[]>> func);
    }

    internal class RedisWrapper : IRedisWrapper
    {
        public IDataSerializer DataSerializer { get; }

        public IDatabase Database { get; set; }

        public string KeyPrefix { get; }

        public string ChannelPrefix { get; }

        public ISubscriber Subscriber { get; set; }

        public RedisWrapper(string keyPrefix)
        {
            KeyPrefix = $"{RedisManager.RedisConfiguration.CachePrefix}/{keyPrefix}";
            ChannelPrefix = $"{RedisManager.RedisConfiguration.ChannelPrefix}/{keyPrefix}";
            DataSerializer = new CompressGZipSerilizer(new JsonDataSerializer());
        }

        public RedisValue Wrap<T>(T t) => DataSerializer.Serialize(t);

        public RedisValue Wrap<T>(Func<T> func) => DataSerializer.Serialize(func());

        public async Task<RedisValue> WrapAsync<T>(Func<Task<T>> func) => DataSerializer.Serialize(await func());

        public T Unwrap<T>(RedisValue redisValue)
        {
            if (redisValue.HasValue)
            {
                try
                {
                    return DataSerializer.Deserializer<T>(redisValue);
                }
                catch (Exception)
                {
                    return redisValue.ToString().JsonToType<T>();
                }
            }
            return default(T);
        }

        public T Unwrap<T>(Func<RedisValue> func) => Unwrap<T>(func());

        public async Task<T> UnwrapAsync<T>(Func<Task<RedisValue>> func)
        {
            var result = await func();
            return Unwrap<T>(result);
        }

        public T[] Unwrap<T>(Func<RedisValue[]> func)
        {
            var result = func();
            return result.Select(Unwrap<T>).ToArray();
        }

        public async Task<T[]> UnwrapAsync<T>(Func<Task<RedisValue[]>> func)
        {
            var result = await func();
            return result.Select(Unwrap<T>).ToArray();
        }

        public bool KeyExists(string key, CommandFlags flags = CommandFlags.None) => Database.KeyExists($"{KeyPrefix}/{key}", flags);

        public Task<bool> KeyExistsAsync(string key, CommandFlags flags = CommandFlags.None) => Database.KeyExistsAsync($"{KeyPrefix}/{key}", flags);

        public bool KeyExpire(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Database.KeyExpire($"{KeyPrefix}/{key}", expiresIn, flags);

        public Task<bool> KeyExpireAsync(string key, TimeSpan? expiresIn, CommandFlags flags = CommandFlags.None) => Database.KeyExpireAsync($"{KeyPrefix}/{key}", expiresIn, flags);

        public bool KeyExpire(string key, DateTime? expiry, CommandFlags flags = CommandFlags.None) => Database.KeyExpire($"{KeyPrefix}/{key}", expiry, flags);

        public Task<bool> KeyExpireAsync(string key, DateTime? expiry, CommandFlags flags = CommandFlags.None) => Database.KeyExpireAsync($"{KeyPrefix}/{key}", expiry, flags);

        public bool KeyDelete(string key, CommandFlags flags = CommandFlags.None) => Database.KeyDelete($"{KeyPrefix}/{key}", flags);

        public Task<bool> KeyDeleteAsync(string key, CommandFlags flags = CommandFlags.None) => Database.KeyDeleteAsync($"{KeyPrefix}/{key}", flags);

        public long KeyDelete(string[] key, CommandFlags flags = CommandFlags.None) => Database.KeyDelete(key.Select(_ => (RedisKey)$"{KeyPrefix}/{_}").ToArray(), flags);

        public Task<long> KeyDeleteAsync(string[] key, CommandFlags flags = CommandFlags.None) => Database.KeyDeleteAsync(key.Select(_ => (RedisKey)$"{KeyPrefix}/{_}").ToArray(), flags);
    }
}
