using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Redis
{
    /// <summary>
    /// RedisConfigurationOption
    /// https://weihanli.github.io/StackExchange.Redis-docs-cn/Configuration.html
    /// </summary>
    public class RedisConfigurationOption
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

        public bool EnableCompress { get; set; } = true;

        public int DefaultDatabase { get; set; }

        public bool Ssl { get; set; }

        public bool AllowAdmin { get; set; }

        public bool AbortOnConnectFail { get; set; } = true;

        public int SyncTimeout { get; set; } = 1000;

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
        /// Optional channel prefix for all pub/sub operations
        /// </summary>
        public string ChannelPrefix { get; set; } = ApplicationHelper.ApplicationName;

        /// <summary>
        /// CachePrefix
        /// </summary>
        public string CachePrefix { get; set; } = ApplicationHelper.ApplicationName;

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
        /// MaxLockCacheExpiry in seconds
        /// 最多锁过期时间（秒），默认 30min
        /// </summary>
        public int MaxLockExpiry { get; set; } = 30 * 60;

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
        public int Port { get; }

        public string Host { get; }

        public RedisServerConfiguration() : this("127.0.0.1")
        {
        }

        public RedisServerConfiguration(string host) : this(host, 6379)
        {
        }

        public RedisServerConfiguration(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException(Resource.InvalidParameter, nameof(host));
            }

            Host = host;
            Port = port;
        }
    }
}
