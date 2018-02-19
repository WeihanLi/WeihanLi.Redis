using System;

namespace WeihanLi.Redis
{
    public static class RedisManager
    {
        internal static RedisConfigurationOption RedisConfiguration { get; set; } = new RedisConfigurationOption();

        #region RedisConfig

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

        #region Hash

        public static IHashClient GetHashClient() => new HashClient();

        #endregion Hash

        #region List

        public static IListClient<T> GetListClient<T>(string listName) => new ListClient<T>(listName);

        #endregion List

        #region Rank

        public static IRankClient GetRankClient() => new RankClient();

        #endregion Rank

        #region Set

        public static ISetClient GetSetClient() => new SetClient();

        #endregion Set

        #region SortedSet

        public static ISortedSetClient GetSortedSetClient() => new SortedSetClient();

        #endregion SortedSet
    }
}
