using System;

namespace WeihanLi.Redis
{
    public static class RedisManager
    {
        internal static RedisConfigurationOption RedisConfiguration { get; set; } = new RedisConfigurationOption();

        public static void AddRedisConfig(Action<RedisConfigurationOption> configAction)
        {
            configAction(RedisConfiguration);
        }

        #region Cache

        public static ICacheClient GetCacheClient()
        {
            return new CacheClient();
        }

        #endregion Cache

        #region Counter

        public static ICounterClient GetCounterClient(string counterName)
        {
            return new CounterClient(counterName);
        }

        public static ICounterClient GetCounterClient(string counterName, long baseCount)
        {
            return new CounterClient(counterName, baseCount);
        }

        public static ICounterClient GetCounterClient(string counterName, TimeSpan? expiresIn)
        {
            return new CounterClient(counterName, expiresIn);
        }

        public static ICounterClient GetCounterClient(string counterName, long baseCount, TimeSpan? expiresIn)
        {
            return new CounterClient(counterName, baseCount, expiresIn);
        }

        public static ICounterClient GetHashCounterClient(string counterName)
        {
            return new HashCounterClient(counterName);
        }

        public static ICounterClient GetHashCounterClient(string counterName, long baseCount)
        {
            return new HashCounterClient(counterName, baseCount);
        }

        public static ICounterClient GetHashCounterClient(string counterName, TimeSpan? expiresIn)
        {
            return new HashCounterClient(counterName, expiresIn);
        }

        public static ICounterClient GetHashCounterClient(string counterName, long baseCount, TimeSpan? expiresIn)
        {
            return new HashCounterClient(counterName, baseCount, expiresIn);
        }

        #endregion Counter

        #region Firewall

        public static IFirewallClient GetFirewallClient()
        {
            return new FirewallClient();
        }

        public static IFirewallClient GetFirewallClient(long limit)
        {
            return new FirewallClient(limit);
        }

        public static IFirewallClient GetHashFirewallClient()
        {
            return new HashFirewallClient();
        }

        public static IFirewallClient GetHashFirewallClient(long limit)
        {
            return new HashFirewallClient(limit);
        }

        #endregion Firewall

        public static IHashClient GetHashClient()
        {
            return new HashClient();
        }

        public static IListClient GetListClient()
        {
            return new ListClient();
        }

        public static IRankClient GetRankClient()
        {
            return new RankClient();
        }

        public static ISetClient GetSetClient()
        {
            return new SetClient();
        }

        public static ISortedSetClient GetSortedSetClient()
        {
            return new SortedSetClient();
        }
    }
}
