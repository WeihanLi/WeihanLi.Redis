using System;
using System.Threading.Tasks;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class HashTest : BaseUnitTest
    {
        [Fact]
        public void HashCounterTest()
        {
            var counterName = "counterTest";
            var counterClient = RedisManager.GetHashCounterClient(counterName, TimeSpan.FromSeconds(10));
            Assert.Equal(0, counterClient.Base);
            Assert.Equal(0, counterClient.Count);
            counterClient.Increase();
            Assert.Equal(1, counterClient.Count);
            counterClient.Increase(5);
            Assert.Equal(6, counterClient.Count);
            counterClient.Decrease(3);
            Assert.Equal(3, counterClient.Count);
            //TODO:BUG fix
            Assert.True(counterClient.Reset());
            Assert.Equal(0, counterClient.Count);
        }

        [Fact]
        public async Task HashFirewallTest()
        {
            var firewallName = "firewallTest";
            var firewallClient = RedisManager.GetHashFirewallClient();
            Assert.True(firewallClient.Hit(firewallName, TimeSpan.FromSeconds(1)));
            //TODO:BUG fix
            Assert.False(firewallClient.Hit(firewallName, TimeSpan.FromSeconds(1)));
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.True(firewallClient.Hit(firewallName, TimeSpan.FromSeconds(1)));
        }
    }
}
