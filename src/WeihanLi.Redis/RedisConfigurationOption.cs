using System;
using System.Collections.Generic;
using StackExchange.Redis;
using WeihanLi.Extensions;

namespace WeihanLi.Redis
{
    /// <summary>
    /// https://weihanli.github.io/StackExchange.Redis-docs-cn/Configuration.html
    /// </summary>
    public class RedisConfigurationOption
    {
        public IReadOnlyList<RedisServerConfiguration> RedisServers { get; set; } = new[]
        {
            new RedisServerConfiguration()
        };

        public string Password { get; set; }

        public int DefaultDatabase { get; set; }

        public bool Ssl { get; set; }

        public bool AllowAdmin { get; set; }

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
        public string ChannelPrefix { get; set; } = "DefaultChannel";

        /// <summary>
        /// CachePrefix
        /// </summary>
        public string CachePrefix { get; set; } = "DefaultProject";
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

    internal class RedisServerConfigurationComparer : IEqualityComparer<RedisServerConfiguration>
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
