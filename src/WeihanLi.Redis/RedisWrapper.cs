using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Extensions;

namespace WeihanLi.Redis
{
    internal interface IRedisWrapper
    {
        /// <summary>
        /// Database
        /// </summary>
        IDatabase Database { get; set; }

        /// <summary>
        /// KeyPrefix
        /// </summary>
        string KeyPrefix { get; }

        T Wrap<T>(Func<RedisValue> func);

        Task<T> WrapAsync<T>(Func<Task<RedisValue>> func);

        T Wrap<T>(string key, Func<string, RedisValue> func);

        Task<T> WrapAsync<T>(string key, Func<string, Task<RedisValue>> func);
    }

    internal class RedisWrapper : IRedisWrapper
    {
        public IDatabase Database { get; set; }

        public string KeyPrefix { get; }

        public RedisWrapper(string keyPrefix)
        {
            KeyPrefix = keyPrefix;
        }

        public T Wrap<T>(string key, Func<string, RedisValue> func)
        {
            var result = func(key);
            return result.HasValue ? (typeof(T).IsPrimitiveType() ? result.ToString().To<T>() : result.ToString().JsonToEntity<T>()) : default(T);
        }

        public async Task<T> WrapAsync<T>(string key, Func<string, Task<RedisValue>> func)
        {
            var result = await func(key);
            return result.HasValue ? (typeof(T).IsPrimitiveType() ? result.To<T>() : result.ToString().JsonToEntity<T>()) : default(T);
        }

        public T Wrap<T>(Func<RedisValue> func)
        {
            var result = func();
            return result.HasValue ? result.ToString().StringToType<T>() : default(T);
        }

        public async Task<T> WrapAsync<T>(Func<Task<RedisValue>> func)
        {
            var result = await func();
            return result.HasValue ? result.ToString().StringToType<T>() : default(T);
        }
    }
}
