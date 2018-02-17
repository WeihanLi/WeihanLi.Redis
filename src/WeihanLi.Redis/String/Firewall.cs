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

        bool Hit(string firewallName);

        bool Hit(string firewallName, TimeSpan? expiresIn);

        Task<bool> HitAsync(string firewallName);

        Task<bool> HitAsync(string firewallName, TimeSpan? expiresIn);
    }

    internal class FirewallClient : BaseRedisClient, IFirewallClient
    {
        internal FirewallClient(long limit) : base(LogHelper.GetLogHelper<FirewallClient>(), new RedisWrapper("String/Firewall/"))
        {
            Limit = limit;
        }

        internal FirewallClient() : this(1)
        {
        }

        public long Limit { get; }

        public bool Hit(string firewallName) => Hit(firewallName, null);

        public bool Hit(string firewallName, TimeSpan? expiresIn)
        {
            var realKey = Wrapper.KeyPrefix + firewallName;
            if (Wrapper.Database.KeyExists(realKey))
            {
                if (Wrapper.Wrap<long>(realKey, k => Wrapper.Database.StringGet(k)) >= Limit)
                {
                    return false;
                }
                Wrapper.Database.StringIncrement(realKey, 1);
            }
            else
            {
                Wrapper.Database.StringSet(realKey, 1, expiresIn, when: StackExchange.Redis.When.NotExists);
            }
            return true;
        }

        public Task<bool> HitAsync(string firewallName)
        => HitAsync(firewallName, null);

        public async Task<bool> HitAsync(string firewallName, TimeSpan? expiresIn)
        {
            var realKey = Wrapper.KeyPrefix + firewallName;
            if (await Wrapper.Database.KeyExistsAsync(realKey))
            {
                if (await Wrapper.WrapAsync<long>(realKey, k => Wrapper.Database.StringGetAsync(k)) >= Limit)
                {
                    return false;
                }
                await Wrapper.Database.StringIncrementAsync(realKey);
            }
            else
            {
                await Wrapper.Database.StringSetAsync(realKey, 1, expiresIn);
            }
            return true;
        }
    }
}
