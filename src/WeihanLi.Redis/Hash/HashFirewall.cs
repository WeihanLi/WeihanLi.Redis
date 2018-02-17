// ReSharper disable once CheckNamespace
using System;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    internal class HashFirewallClient : BaseRedisClient, IFirewallClient
    {
        private const string KeyName = "__firewall";

        internal HashFirewallClient() : this(1)
        {
        }

        internal HashFirewallClient(long limit) : base(LogHelper.GetLogHelper<FirewallClient>(), new RedisWrapper("Hash/Firewall/"))
        {
            Limit = limit;
        }

        public long Limit { get; }

        public bool Hit(string firewallName) => Hit(firewallName, null);

        public bool Hit(string firewallName, TimeSpan? expiresIn)
        {
            var fieldName = Wrapper.KeyPrefix + firewallName;
            if (Wrapper.Database.KeyExists($"{KeyName}:{fieldName}"))
            {
                if (Wrapper.Wrap<long>(() => Wrapper.Database.HashGet(KeyName, fieldName)) >= Limit)
                {
                    return false;
                }
                Wrapper.Database.HashIncrement(KeyName, fieldName);
            }
            else
            {
                Wrapper.Database.HashSet(KeyName, fieldName, 1);
                Wrapper.Database.KeyExpire($"{KeyName}:{fieldName}", expiresIn);
            }
            return true;
        }

        public Task<bool> HitAsync(string firewallName)
        => HitAsync(firewallName, null);

        public async Task<bool> HitAsync(string firewallName, TimeSpan? expiresIn)
        {
            var fieldName = Wrapper.KeyPrefix + firewallName;
            if (await Wrapper.Database.KeyExistsAsync($"{KeyName}:{fieldName}"))
            {
                if (await Wrapper.WrapAsync<long>(() => Wrapper.Database.HashGetAsync(KeyName, fieldName)) >= Limit)
                {
                    return false;
                }
                await Wrapper.Database.HashIncrementAsync(KeyName, fieldName);
            }
            else
            {
                await Task.WhenAll(Wrapper.Database.HashSetAsync(KeyName, fieldName, 1), Wrapper.Database.KeyExpireAsync($"{KeyName}:{fieldName}", expiresIn));
            }
            return true;
        }
    }
}
