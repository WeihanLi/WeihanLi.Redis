using System;
using System.Collections.Generic;
using System.Threading;

namespace WeihanLi.Redis
{
    public static class RedisManager
    {
        internal static RedisConfigurationOption RedisConfiguration { get; } = new RedisConfigurationOption();

        private static readonly Lazy<IReadOnlyDictionary<RedisDataType, CommonRedisClient>> CommonRedisClients = new Lazy<IReadOnlyDictionary<RedisDataType, CommonRedisClient>>(() => new Dictionary<RedisDataType, CommonRedisClient>
        {
            { RedisDataType.Cache, new CommonRedisClient(RedisDataType.Cache) },
            { RedisDataType.Counter, new CommonRedisClient(RedisDataType.Counter) },
            { RedisDataType.Firewall, new CommonRedisClient(RedisDataType.Firewall) },
            { RedisDataType.RedLock, new CommonRedisClient(RedisDataType.RedLock) },

            { RedisDataType.Hash, new CommonRedisClient(RedisDataType.Hash) },
            { RedisDataType.Dictionary, new CommonRedisClient(RedisDataType.Dictionary) },

            { RedisDataType.List, new CommonRedisClient(RedisDataType.List) },
            { RedisDataType.Set, new CommonRedisClient(RedisDataType.Set) },
            { RedisDataType.SortedSet, new CommonRedisClient(RedisDataType.SortedSet) },
            { RedisDataType.Rank, new CommonRedisClient(RedisDataType.Rank) }
        }, LazyThreadSafetyMode.PublicationOnly);

        #region RedisConfig

        /// <summary>
        /// 设置Redis配置
        /// </summary>
        /// <param name="configAction">configAction</param>
        public static void AddRedisConfig(Action<RedisConfigurationOption> configAction) => configAction(RedisConfiguration);

        #endregion RedisConfig

        #region Common

        /// <summary>
        /// GetCommonRedisClient
        /// </summary>
        /// <param name="redisDataType">redisDataType</param>
        /// <returns></returns>
        public static ICommonRedisClient GetCommonRedisClient(RedisDataType redisDataType) => CommonRedisClients.Value[redisDataType];

        #endregion Common

        #region Cache

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

        #endregion RedisLock

        #region Hash

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
            => GetDictionaryClient<TKey, TValue>(keyName, TimeSpan.FromDays(1));

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
        /// <param name="isSlidingExpiry">滑动过期时间</param>
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

        #region Rank

        public static IRankClient GetRankClient() => new RankClient();

        #endregion Rank

        #region Set

        public static ISetClient<T> GetSetClient<T>(string keyName) => new SetClient<T>(keyName);

        #endregion Set

        #region SortedSet

        public static ISortedSetClient<T> GetSortedSetClient<T>(string keyName) => new SortedSetClient<T>(keyName);

        #endregion SortedSet

        #region PubSub

        public static IPubSubClient GetPubSubClient() => new PubSubClient();

        #endregion PubSub
    }
}
