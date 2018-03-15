using System;
using System.Threading.Tasks;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class StringTest : BaseUnitTest
    {
        [Fact]
        public void StringCacheTest()
        {
            var key = "test111";
            var value = "Hello WeihanLi.Redis";
            var cacheClient = RedisManager.CacheClient;
            var result = cacheClient.Set(key, value);
            Assert.True(cacheClient.Exists(key));
            Assert.Equal(value, cacheClient.Get(key));
            Assert.True(cacheClient.Remove(key));
            Assert.False(cacheClient.Exists(key));
        }

        [Fact]
        public void StringCounterTest()
        {
            var counterName = "counterTest";
            var counterClient = RedisManager.GetCounterClient(counterName, TimeSpan.FromSeconds(60));
            Assert.Equal(0, counterClient.Base);
            Assert.Equal(0, counterClient.Count());
            counterClient.Increase();
            Assert.Equal(1, counterClient.Count());
            counterClient.Increase(5);
            Assert.Equal(6, counterClient.Count());
            counterClient.Decrease(3);
            Assert.Equal(3, counterClient.Count());
            Assert.True(counterClient.Reset());
            Assert.Equal(0, counterClient.Count());
        }

        [Fact]
        public async Task StringFirewallTest()
        {
            var firewallName = "firewallTest";
            var firewallClient = RedisManager.GetFirewallClient(firewallName, TimeSpan.FromSeconds(3));
            Assert.True(firewallClient.Hit());
            Assert.False(firewallClient.Hit());
            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.True(firewallClient.Hit());
        }

        [Fact]
        public void RedisLock()
        {
            using (var client = RedisManager.GetRedLockClient("redLockTest"))
            {
                Assert.True(client.TryLock(TimeSpan.FromSeconds(10)));
                using (var client1 = RedisManager.GetRedLockClient("redLockTest"))
                {
                    Assert.False(client.TryLock(TimeSpan.FromSeconds(10)));
                    Assert.False(client1.Release());
                }
                Assert.True(client.Release());
            }

            var key = Guid.NewGuid().ToString("N");
            using (var client = RedisManager.GetRedLockClient(key))
            {
                Assert.True(client.TryLock(TimeSpan.FromSeconds(20)));
            }

            using (var client = RedisManager.GetRedLockClient(key))
            {
                Assert.True(client.TryLock(TimeSpan.FromMinutes(3)));
                Assert.True(client.Release());
            }
        }
    }
}
