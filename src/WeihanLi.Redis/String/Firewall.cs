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
            if (Wrapper.Database.KeyExists(_firewallName))
            {
                if (Convert.ToInt64(Wrapper.Database.StringGet(_firewallName)) >= Limit)
                {
                    return false;
                }
                Wrapper.Database.StringIncrement(_firewallName);
            }
            else
            {
                Wrapper.Database.StringSet(_firewallName, 1, _expiresIn, StackExchange.Redis.When.NotExists);
            }
            return true;
        }

        public async Task<bool> HitAsync()
        {
            if (await Wrapper.Database.KeyExistsAsync(_firewallName))
            {
                if (Convert.ToInt64(await Wrapper.Database.StringGetAsync(_firewallName)) >= Limit)
                {
                    return false;
                }
                await Wrapper.Database.StringIncrementAsync(_firewallName);
            }
            else
            {
                await Wrapper.Database.StringSetAsync(_firewallName, 1, _expiresIn, StackExchange.Redis.When.NotExists);
            }
            return true;
        }
    }
}
