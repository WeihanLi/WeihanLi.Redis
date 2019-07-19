using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Redis.Internals;

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

        internal FirewallClient(string firewallName, long limit, TimeSpan? expiresIn, ILogger<FirewallClient> logger) : base(logger, new RedisWrapper(RedisConstants.FirewallPrefix))
        {
            _firewallName = Wrapper.GetRealKey(firewallName);
            if (expiresIn.HasValue)
            {
                Wrapper.Database.StringSet(_firewallName, 0, expiresIn, When.NotExists);
            }
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

        public bool Hit() => Wrapper.Database.StringIncrement(_firewallName) <= Limit;

        public async Task<bool> HitAsync() => await Wrapper.Database.StringIncrementAsync(_firewallName).ContinueWith(r => r.Result <= Limit);
    }
}
