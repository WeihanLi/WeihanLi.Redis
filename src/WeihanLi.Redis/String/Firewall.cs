using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly TimeSpan? _expiresIn;
        private readonly string _firewallName;

        internal FirewallClient(string firewallName, long limit, TimeSpan? expiresIn, ILogger<FirewallClient> logger) : base(logger, new RedisWrapper(RedisConstants.FirewallPrefix))
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
            if (Wrapper.Database.Value.KeyExists(_firewallName))
            {
                if (Convert.ToInt64(Wrapper.Database.Value.StringGet(_firewallName)) >= Limit)
                {
                    return false;
                }
                Wrapper.Database.Value.StringIncrement(_firewallName);
            }
            else
            {
                Wrapper.Database.Value.StringSet(_firewallName, 1, _expiresIn, StackExchange.Redis.When.NotExists);
            }
            return true;
        }

        public async Task<bool> HitAsync()
        {
            if (await Wrapper.Database.Value.KeyExistsAsync(_firewallName))
            {
                if (Convert.ToInt64(await Wrapper.Database.Value.StringGetAsync(_firewallName)) >= Limit)
                {
                    return false;
                }
                await Wrapper.Database.Value.StringIncrementAsync(_firewallName);
            }
            else
            {
                await Wrapper.Database.Value.StringSetAsync(_firewallName, 1, _expiresIn, StackExchange.Redis.When.NotExists);
            }
            return true;
        }
    }
}
