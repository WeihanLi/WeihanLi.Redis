using System;
using System.Collections.Concurrent;
using WeihanLi.Redis.List;

#if NETSTANDARD2_0

using Microsoft.Extensions.DependencyInjection;

#endif

namespace WeihanLi.Redis
{
    public static class RedisManager
    {
        internal static readonly RedisConfigurationOption RedisConfiguration = new RedisConfigurationOption();

        private static readonly ConcurrentDictionary<RedisDataType, CommonRedisClient> CommonRedisClients = new ConcurrentDictionary<RedisDataType, CommonRedisClient>();

        private static readonly ConcurrentDictionary<Type, IRedisClient> SingletonRedisClients = new ConcurrentDictionary<Type, IRedisClient>();

        /// <summary>
        /// Version of WeihanLi.Redis
        /// </summary>
        public static Version Version => typeof(RedisManager).Assembly.GetName().Version;

        #region RedisConfig

        /// <summary>
        /// 设置Redis配置
        /// </summary>
        /// <param name="configAction">configAction</param>
        public static void AddRedisConfig(Action<RedisConfigurationOption> configAction) => configAction(RedisConfiguration);

#if NETSTANDARD2_0

        public static IServiceCollection AddRedisConfig(this IServiceCollection serviceCollection, Action<RedisConfigurationOption> configAction)
        {
            configAction(RedisConfiguration);
            serviceCollection.Configure(configAction);
            serviceCollection.AddSingleton<ICacheClient, CacheClient>();
            serviceCollection.AddSingleton<IHashClient, HashClient>();
            serviceCollection.AddSingleton<IPubSubClient, PubSubClient>();
            return serviceCollection;
        }

#endif

        #endregion RedisConfig

        #region Common

        /// <summary>
        /// GetCommonRedisClient
        /// </summary>
        /// <param name="redisDataType">redisDataType</param>
        /// <returns></returns>
        public static ICommonRedisClient GetCommonRedisClient(RedisDataType redisDataType) => CommonRedisClients.GetOrAdd(redisDataType, type => new CommonRedisClient(type));

        #endregion Common

        #region Cache

        public static ICacheClient CacheClient =>
            SingletonRedisClients.GetOrAdd(typeof(ICacheClient), t => new CacheClient()) as ICacheClient;

        [Obsolete("Please use RedisManager.CacheClient", true)]
        public static ICacheClient GetCacheClient() => new CacheClient();

        #endregion Cache

        #region Counter

        public static ICounterClient GetCounterClient(string counterName) => new CounterClient(counterName);

        public static ICounterClient GetCounterClient(string counterName, long baseCount) => new CounterClient(counterName, baseCount);

        public static ICounterClient GetCounterClient(string counterName, TimeSpan? expiresIn) => new CounterClient(counterName, expiresIn);

        public static ICounterClient GetCounterClient(string counterName, long baseCount, TimeSpan? expiresIn) => new CounterClient(counterName, baseCount, expiresIn);

        #endregion Counter

        #region Firewall

        public static IFirewallClient GetFirewallClient(string firewallName) => new FirewallClient(firewallName);

        public static IFirewallClient GetFirewallClient(string firewallName, long limit) => new FirewallClient(firewallName, limit);

        public static IFirewallClient GetFirewallClient(string firewallName, TimeSpan? expiresIn) => new FirewallClient(firewallName, expiresIn);

        public static IFirewallClient GetFirewallClient(string firewallName, long limit, TimeSpan? expiresIn) => new FirewallClient(firewallName, limit, expiresIn);

        #endregion Firewall

        #region RedisLock

        /// <summary>
        /// RedisLock
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static IRedLockClient GetRedLockClient(string key) => new RedLockClient(key);

        /// <summary>
        /// RedisLock
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="maxRetryCount">maxRetryCount</param>
        /// <returns></returns>
        public static IRedLockClient GetRedLockClient(string key, int maxRetryCount) => new RedLockClient(key, maxRetryCount);

        #endregion RedisLock

        #region Hash

        public static IHashClient HashClient => SingletonRedisClients.GetOrAdd(typeof(IHashClient), (t) => new HashClient()) as IHashClient;

        [Obsolete("Please use RedisManager.HashClient", true)]
        public static IHashClient GetHashClient() => new HashClient();

        #endregion Hash

        #region Dictionary

        /// <summary>
        /// 获取一个 DictionaryClient
        /// 默认过期时间滑动过期一天
        /// </summary>
        /// <typeparam name="TKey">TKey</typeparam>
        /// <typeparam name="TValue">TValue</typeparam>
        /// <param name="keyName">keyName</param>
        /// <returns></returns>
        public static IDictionaryClient<TKey, TValue> GetDictionaryClient<TKey, TValue>(string keyName)
            => GetDictionaryClient<TKey, TValue>(keyName, (TimeSpan?)null);

        /// <summary>
        /// 获取一个 DictionaryClient
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="keyName">keyName</param>
        /// <param name="expiry">过期时间(滑动过期)</param>
        /// <returns></returns>
        public static IDictionaryClient<TKey, TValue> GetDictionaryClient<TKey, TValue>(string keyName, TimeSpan? expiry) => new DictionaryClient<TKey, TValue>(keyName, expiry);

        /// <summary>
        /// 获取一个 DictionaryClient
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="keyName">keyName</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="isSlidingExpiry">更新是否重置过期时间</param>
        /// <returns></returns>
        public static IDictionaryClient<TKey, TValue> GetDictionaryClient<TKey, TValue>(string keyName, TimeSpan? expiry, bool isSlidingExpiry) => new DictionaryClient<TKey, TValue>(keyName, expiry, isSlidingExpiry);

        /// <summary>
        /// 获取一个 DictionaryClient
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="keyName">keyName</param>
        /// <param name="expiry">绝对过期时间</param>
        /// <returns></returns>
        public static IDictionaryClient<TKey, TValue> GetDictionaryClient<TKey, TValue>(string keyName, DateTime? expiry) => new DictionaryClient<TKey, TValue>(keyName, expiry);

        #endregion Dictionary

        #region List

        public static IListClient<T> GetListClient<T>(string keyName) => new ListClient<T>(keyName);

        #endregion List

        #region Queue

        public static IQueueClient<T> GetQueueClient<T>(string keyName) => new QueueClient<T>(keyName);

        #endregion Queue

        #region Stack

        public static IStackClient<T> GetStackClient<T>(string keyName) => new StackClient<T>(keyName);

        #endregion Stack

        #region Rank

        public static IRankClient<T> GetRankClient<T>(string rankName) => new RankClient<T>(rankName);

        #endregion Rank

        #region Set

        public static ISetClient<T> GetSetClient<T>(string keyName) => new SetClient<T>(keyName);

        #endregion Set

        #region SortedSet

        public static ISortedSetClient<T> GetSortedSetClient<T>(string keyName) => new SortedSetClient<T>(keyName);

        #endregion SortedSet

        #region PubSub

        public static IPubSubClient PubSubClient => SingletonRedisClients.GetOrAdd(typeof(IPubSubClient), t => new PubSubClient()) as IPubSubClient;

        [Obsolete("Please use RedisManager.PubSubClient", true)]
        public static IPubSubClient GetPubSubClient() => new PubSubClient();

        #endregion PubSub
    }
}
