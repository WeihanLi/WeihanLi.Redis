// ReSharper disable once CheckNamespace
using System;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    /// <summary>
    /// FirewallClient
    /// </summary>
    public interface IFirewallClient
    {
        long Limit { get; }

        bool Hit();

        Task<bool> HitAsync();
    }

    internal class FirewallClient : BaseRedisClient, IFirewallClient
    {
        private readonly TimeSpan? _expiresIn;
        private readonly string _firewallName;

        internal FirewallClient(string firewallName, long limit, TimeSpan? expiresIn) : base(LogHelper.GetLogHelper<FirewallClient>(), new RedisWrapper("String/Firewall/"))
        {
            _firewallName = Wrapper.KeyPrefix + firewallName;
            _expiresIn = expiresIn;
            Limit = limit;
        }

        internal FirewallClient(string firewallName, TimeSpan? expiresIn) : this(firewallName, 1, expiresIn)
        {
        }

        internal FirewallClient(string firewallName, long limit) : this(firewallName, limit, null)
        {
        }

        internal FirewallClient(string firewallName) : this(firewallName, 1)
        {
        }

        public long Limit { get; }

        public bool Hit()
        {
            if (Wrapper.Database.KeyExists(_firewallName))
            {
                if (Wrapper.Wrap<long>(_firewallName, k => Wrapper.Database.StringGet(k)) >= Limit)
                {
                    return false;
                }
                Wrapper.Database.StringIncrement(_firewallName, 1);
            }
            else
            {
                Wrapper.Database.StringSet(_firewallName, 1, _expiresIn, when: StackExchange.Redis.When.NotExists);
            }
            return true;
        }

        public async Task<bool> HitAsync()
        {
            if (await Wrapper.Database.KeyExistsAsync(_firewallName))
            {
                if (await Wrapper.WrapAsync<long>(_firewallName, k => Wrapper.Database.StringGetAsync(k)) >= Limit)
                {
                    return false;
                }
                await Wrapper.Database.StringIncrementAsync(_firewallName);
            }
            else
            {
                await Wrapper.Database.StringSetAsync(_firewallName, 1, _expiresIn);
            }
            return true;
        }
    }
}
