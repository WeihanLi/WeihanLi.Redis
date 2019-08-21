using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    /// <summary>
    /// FirewallClient
    /// </summary>
    public interface IFirewallClient : IRedisClient
    {
        long Limit { get; }

        bool Hit();

        Task<bool> HitAsync();
    }

    internal class FirewallClient : BaseRedisClient, IFirewallClient
    {
        private readonly string _firewallName;
        private readonly TimeSpan? _expiresIn;

        internal FirewallClient(string firewallName, long limit, TimeSpan? expiresIn, ILogger<FirewallClient> logger)
            : base(logger, new RedisWrapper(RedisDataType.Firewall))
        {
            _firewallName = Wrapper.GetRealKey(firewallName);
            _expiresIn = expiresIn;
            Limit = limit;
        }

        internal FirewallClient(string firewallName, TimeSpan? expiresIn, ILogger<FirewallClient> logger) : this(firewallName, 1, expiresIn, logger)
        {
        }

        internal FirewallClient(string firewallName, long limit, ILogger<FirewallClient> logger) : this(firewallName, limit, null, logger)
        {
        }

        internal FirewallClient(string firewallName, ILogger<FirewallClient> logger) : this(firewallName, 1, logger)
        {
        }

        public long Limit { get; }

        public bool Hit()
        {
            if (_expiresIn.HasValue)
            {
                Wrapper.Database.StringSet(_firewallName, 0, _expiresIn, When.NotExists);
            }

            return Wrapper.Database.StringIncrement(_firewallName) <= Limit;
        }

        public async Task<bool> HitAsync()
        {
            if (_expiresIn.HasValue)
            {
                await Wrapper.Database.StringSetAsync(_firewallName, 0, _expiresIn, When.NotExists);
            }
            return await Wrapper.Database.StringIncrementAsync(_firewallName).ContinueWith(r => r.Result <= Limit);
        }
    }
}
