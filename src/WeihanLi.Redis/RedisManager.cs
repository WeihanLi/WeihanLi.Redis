using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;
using WeihanLi.Redis.List;

namespace WeihanLi.Redis
{
    public static class RedisManager
    {
        internal static readonly RedisConfigurationOptions RedisConfiguration = new RedisConfigurationOptions();

        private static readonly ConcurrentDictionary<RedisDataType, CommonRedisClient> CommonRedisClients = new ConcurrentDictionary<RedisDataType, CommonRedisClient>();

        /// <summary>
        /// Version of WeihanLi.Redis
        /// </summary>
        public static Version Version => typeof(RedisManager).Assembly.GetName().Version;

        #region RedisConfig

        public static IServiceCollection AddRedisConfig(this IServiceCollection serviceCollection, Action<RedisConfigurationOptions> configAction)
        {
            configAction(RedisConfiguration);
            serviceCollection.Configure(configAction);

            var configurationOptions = new ConfigurationOptions
            {
                Password = RedisManager.RedisConfiguration.Password,
                DefaultDatabase = RedisManager.RedisConfiguration.DefaultDatabase,
                ConnectRetry = RedisManager.RedisConfiguration.ConnectRetry,
                ConnectTimeout = RedisManager.RedisConfiguration.ConnectTimeout,
                AllowAdmin = RedisManager.RedisConfiguration.AllowAdmin,
                Ssl = RedisManager.RedisConfiguration.Ssl,
                Proxy = RedisManager.RedisConfiguration.Proxy,
                AbortOnConnectFail = RedisManager.RedisConfiguration.AbortOnConnectFail,
                SyncTimeout = RedisConfiguration.SyncTimeout
            };
            configurationOptions.EndPoints.AddRange(RedisConfiguration.RedisServers.Select(s => ConvertHelper.ToEndPoint(s.Host, s.Port)).ToArray());

            serviceCollection.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configurationOptions));

            serviceCollection.TryAddSingleton<ICacheClient, CacheClient>();
            serviceCollection.TryAddSingleton<IHashClient, HashClient>();
            serviceCollection.TryAddSingleton<IPubSubClient, PubSubClient>();
            serviceCollection.AddSingleton<IDataSerializer, JsonDataSerializer>();
            serviceCollection.AddSingleton<ICompressSerializer, CompressGZipSerilizer>();

            serviceCollection.AddLogging();

            return serviceCollection;
        }

        #endregion RedisConfig

        #region Common

        /// <summary>
        /// GetCommonRedisClient
        /// </summary>
        /// <param name="redisDataType">redisDataType</param>
        /// <returns></returns>
        public static ICommonRedisClient GetCommonRedisClient(RedisDataType redisDataType) => CommonRedisClients.GetOrAdd(redisDataType,
            type => new CommonRedisClient(type, DependencyResolver.Current.ResolveService<ILogger<CommonRedisClient>>()));

        #endregion Common

        #region Cache

        public static ICacheClient CacheClient => DependencyResolver.Current.GetRequiredService<ICacheClient>();

        #endregion Cache

        #region Counter

        public static ICounterClient GetCounterClient(string counterName) => new CounterClient(counterName, DependencyResolver.Current.ResolveService<ILogger<CounterClient>>());

        public static ICounterClient GetCounterClient(string counterName, long baseCount) => new CounterClient(counterName, baseCount, DependencyResolver.Current.ResolveService<ILogger<CounterClient>>());

        public static ICounterClient GetCounterClient(string counterName, TimeSpan? expiresIn) => new CounterClient(counterName, expiresIn, DependencyResolver.Current.ResolveService<ILogger<CounterClient>>());

        public static ICounterClient GetCounterClient(string counterName, long baseCount, TimeSpan? expiresIn) => new CounterClient(counterName, baseCount, expiresIn, DependencyResolver.Current.ResolveService<ILogger<CounterClient>>());

        #endregion Counter

        #region Firewall

        public static IFirewallClient GetFirewallClient(string firewallName) => new FirewallClient(firewallName, DependencyResolver.Current.ResolveService<ILogger<FirewallClient>>());

        public static IFirewallClient GetFirewallClient(string firewallName, long limit) => new FirewallClient(firewallName, limit, DependencyResolver.Current.ResolveService<ILogger<FirewallClient>>());

        public static IFirewallClient GetFirewallClient(string firewallName, TimeSpan? expiresIn) => new FirewallClient(firewallName, expiresIn, DependencyResolver.Current.ResolveService<ILogger<FirewallClient>>());

        public static IFirewallClient GetFirewallClient(string firewallName, long limit, TimeSpan? expiresIn) => new FirewallClient(firewallName, limit, expiresIn, DependencyResolver.Current.ResolveService<ILogger<FirewallClient>>());

        #endregion Firewall

        #region RedisLock

        /// <summary>
        /// RedisLock
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static IRedLockClient GetRedLockClient(string key) => new RedLockClient(key, DependencyResolver.Current.ResolveService<ILogger<RedLockClient>>());

        /// <summary>
        /// RedisLock
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="maxRetryCount">maxRetryCount</param>
        /// <returns></returns>
        public static IRedLockClient GetRedLockClient(string key, int maxRetryCount) => new RedLockClient(key, maxRetryCount, DependencyResolver.Current.ResolveService<ILogger<RedLockClient>>());

        #endregion RedisLock

        #region Hash

        public static IHashClient HashClient
            => DependencyResolver.Current.GetRequiredService<IHashClient>();

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
        public static IDictionaryClient<TKey, TValue> GetDictionaryClient<TKey, TValue>(string keyName, TimeSpan? expiry) => new DictionaryClient<TKey, TValue>(keyName, expiry, DependencyResolver.Current.ResolveService<ILogger<DictionaryClient<TKey, TValue>>>());

        /// <summary>
        /// 获取一个 DictionaryClient
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="keyName">keyName</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="isSlidingExpiry">更新是否重置过期时间</param>
        /// <returns></returns>
        public static IDictionaryClient<TKey, TValue> GetDictionaryClient<TKey, TValue>(string keyName, TimeSpan? expiry, bool isSlidingExpiry) => new DictionaryClient<TKey, TValue>(keyName, expiry, isSlidingExpiry, DependencyResolver.Current.ResolveService<ILogger<DictionaryClient<TKey, TValue>>>());

        /// <summary>
        /// 获取一个 DictionaryClient
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="keyName">keyName</param>
        /// <param name="expiry">绝对过期时间</param>
        /// <returns></returns>
        public static IDictionaryClient<TKey, TValue> GetDictionaryClient<TKey, TValue>(string keyName, DateTime? expiry) => new DictionaryClient<TKey, TValue>(keyName, expiry, DependencyResolver.Current.ResolveService<ILogger<DictionaryClient<TKey, TValue>>>());

        #endregion Dictionary

        #region List

        public static IListClient<T> GetListClient<T>(string keyName) => new ListClient<T>(keyName, DependencyResolver.Current.ResolveService<ILogger<ListClient<T>>>());

        #endregion List

        #region Queue

        public static IQueueClient<T> GetQueueClient<T>(string keyName) => new QueueClient<T>(keyName, DependencyResolver.Current.ResolveService<ILogger<QueueClient<T>>>());

        #endregion Queue

        #region Stack

        public static IStackClient<T> GetStackClient<T>(string keyName) => new StackClient<T>(keyName, DependencyResolver.Current.ResolveService<ILogger<StackClient<T>>>());

        #endregion Stack

        #region Rank

        public static IRankClient<T> GetRankClient<T>(string rankName) => new RankClient<T>(rankName, DependencyResolver.Current.ResolveService<ILogger<RankClient<T>>>());

        #endregion Rank

        #region Set

        public static ISetClient<T> GetSetClient<T>(string keyName) => new SetClient<T>(keyName, DependencyResolver.Current.ResolveService<ILogger<SetClient<T>>>());

        #endregion Set

        #region SortedSet

        public static ISortedSetClient<T> GetSortedSetClient<T>(string keyName) => new SortedSetClient<T>(keyName, DependencyResolver.Current.ResolveService<ILogger<SortedSetClient<T>>>());

        #endregion SortedSet

        #region PubSub

        public static IPubSubClient PubSubClient
            => DependencyResolver.Current.GetRequiredService<IPubSubClient>();

        #endregion PubSub
    }
}
