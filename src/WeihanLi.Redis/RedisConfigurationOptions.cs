using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Redis
{
    /// <summary>
    /// RedisConfigurationOption
    /// https://weihanli.github.io/StackExchange.Redis-docs-zh-cn/Configuration.html
    /// </summary>
    public class RedisConfigurationOptions
    {
        private IReadOnlyCollection<RedisServerConfiguration> _redisServers = new[]
        {
            new RedisServerConfiguration()
        };

        private string _keySeparator = ":";

        public IReadOnlyCollection<RedisServerConfiguration> RedisServers
        {
            get => _redisServers;
            set => _redisServers = value.Distinct(new RedisServerConfigurationComparer()).ToArray();
        }

        public string Password { get; set; }

        /// <summary>
        /// Identification for the connection within redis
        /// </summary>
        public string ClientName { get; set; } = $"{ApplicationHelper.ApplicationName}-{Environment.MachineName}";

        /// <summary>
        /// Optional channel prefix for all pub/sub operations
        /// </summary>
        public string ChannelPrefix { get; set; } = "__WeihanLi.Redis.PubSub";

        /// <summary>
        /// EventBus channel prefix
        /// </summary>
        public string EventBusChannelPrefix { get; set; } = "__WeihanLi.Redis.EventBus";

        /// <summary>
        /// EventStore CachePrefix
        /// </summary>
        public string EventStoreCacheKey { get; set; } = "WeihanLi.Redis.Events";

        /// <summary>
        /// EventQueue CachePrefix
        /// </summary>
        public string EventQueueStorePrefix { get; set; } = "WeihanLi.Redis.EventQueue";

        /// <summary>
        /// CachePrefix
        /// </summary>
        public string CachePrefix { get; set; } = ApplicationHelper.ApplicationName;

        /// <summary>
        /// KeySeparator
        /// </summary>
        public string KeySeparator
        {
            get => _keySeparator;
            set
            {
                if (value.IsNotNullOrWhiteSpace())
                {
                    _keySeparator = value;
                }
            }
        }

        public bool EnableCompress { get; set; }

        public bool EnableRandomExpiry { get; set; }

        public int DefaultDatabase { get; set; }

        public bool Ssl { get; set; }

        public bool AllowAdmin { get; set; }

        public bool AbortOnConnectFail { get; set; }

        public int SyncTimeout { get; set; } = 5000;

        public int AsyncTimeout { get; set; } = 5000;

        public int ConnectRetry { get; set; } = 3;

        /// <summary>
        /// Timeout (ms) for connect operations
        /// </summary>
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>
        /// Type of proxy in use (if any); for example “twemproxy”
        /// </summary>
        public Proxy Proxy { get; set; } = Proxy.None;

        /// <summary>
        /// A slightly unusual feature of redis is that you can disable and/or rename individual commands
        /// https://github.com/StackExchange/StackExchange.Redis/blob/master/docs/Configuration.md#renaming-commands
        /// </summary>
        public Dictionary<string, string> CommandMap { get; set; }

        /// <summary>
        /// AllowNoExpiry
        /// 是否允许永不过期
        /// </summary>
        public bool AllowNoExpiry { get; set; } = true;

        /// <summary>
        /// MaxCacheExpiry
        /// 缓存最长过期时间，默认30天
        /// </summary>
        public TimeSpan MaxCacheExpiry { get; set; } = TimeSpan.FromDays(30);

        /// <summary>
        /// MaxRandomCacheExpiry(seconds)
        /// 最多随机缓存过期时间(缓存雪崩)秒
        /// </summary>
        public int MaxRandomCacheExpiry { get; set; } = 10;

        /// <summary>
        /// LockRetryDelay in seconds
        /// 锁重试延迟时间（毫秒）
        /// </summary>
        public int LockRetryDelay { get; set; } = 400;

        /// <summary>
        /// MaxLockRetryTime in seconds
        /// 获取锁最长时间（秒），默认3 min
        /// </summary>
        public int MaxLockRetryTime { get; set; } = 180;

        /// <summary>
        /// MaxLockCacheExpiry in seconds
        /// 最多锁过期时间（秒），默认 30min
        /// </summary>
        public int MaxLockExpiry { get; set; } = 1800;

        /// <summary>
        /// default redis server version
        /// 默认 redis-server 版本
        /// </summary>
        public Version DefaultVersion { get; set; }

        /// <summary>
        /// save null into redis
        /// 是否要保存 null
        /// </summary>
        public bool EnableNullValue { get; set; }

        private class RedisServerConfigurationComparer : IEqualityComparer<RedisServerConfiguration>
        {
            public bool Equals(RedisServerConfiguration x, RedisServerConfiguration y)
            {
                if (null == x || null == y)
                {
                    return false;
                }
                return x.Host.EqualsIgnoreCase(y.Host) && x.Port == y.Port;
            }

            public int GetHashCode(RedisServerConfiguration obj)
            {
                return $"{obj.Host}:{obj.Port}".GetHashCode();
            }
        }
    }

    public class RedisServerConfiguration
    {
        private const int DefaultRedisPort = 6379;

        public int Port { get; }

        public string Host { get; }

        public RedisServerConfiguration() : this("127.0.0.1", DefaultRedisPort)
        {
        }

        public RedisServerConfiguration(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(Resource.InvalidParameter, nameof(host));
            }

            var lastIndex = host.LastIndexOf(':');
            if (lastIndex > 0 && host.Length > (lastIndex + 1))
            {
                if (int.TryParse(host.Substring(lastIndex + 1), out var port))
                {
                    Host = host.Substring(0, lastIndex);
                    Port = port;
                }
            }

            if (string.IsNullOrEmpty(Host))
            {
                Host = host;
                Port = DefaultRedisPort;
            }
        }

        public RedisServerConfiguration(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(Resource.InvalidParameter, nameof(host));
            }

            Host = host;
            Port = port;
        }
    }
}
