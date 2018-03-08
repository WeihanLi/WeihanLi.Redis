using System;

namespace WeihanLi.Redis
{
    public static class RedisManager
    {
        internal static RedisConfigurationOption RedisConfiguration { get; set; } = new RedisConfigurationOption();

        #region RedisConfig

        /// <summary>
        /// 设置Redis配置
        /// </summary>
        /// <param name="configAction">configAction</param>
        public static void AddRedisConfig(Action<RedisConfigurationOption> configAction) => configAction(RedisConfiguration);

        #endregion RedisConfig

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
