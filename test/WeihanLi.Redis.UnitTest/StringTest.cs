using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.False(cacheClient.Exists(key));
            Assert.Null(cacheClient.Get(key));
            Assert.True(cacheClient.Set(key, 1));
            Assert.True(cacheClient.Set(key, value));
            Assert.True(cacheClient.Exists(key));
            Assert.Equal(value, cacheClient.Get(key));
            Assert.True(cacheClient.Remove(key));
            Assert.False(cacheClient.Exists(key));
            Assert.Null(cacheClient.GetOrSet("testGetOrSet", () => (string)null, TimeSpan.FromSeconds(20)));
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

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        public async Task CounterConcurrentTest(int taskCount)
        {
            var counterName = "concurrentCounterTest";

            RedisManager.GetCommonRedisClient(RedisDataType.Counter).KeyDelete(counterName);
            var tasks = new List<Task>();
            Func<Task> func = () =>
            {
                var counter = RedisManager.GetCounterClient(counterName, TimeSpan.FromMinutes(1));
                return counter.IncreaseAsync();
            };
            for (var i = 0; i < taskCount; i++)
            {
                tasks.Add(func());
            }
            await Task.WhenAll(tasks);

            Assert.Equal(taskCount, RedisManager.GetCounterClient(counterName, TimeSpan.FromMinutes(1)).Count());
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

        [Theory]
        [InlineData(1, 10)]
        [InlineData(3, 20)]
        public async Task StringFirewallConcurrentTest(int limit, int taskCount)
        {
            var firewallName = "concurrentFirewallTest";
            var tasks = new List<Task<bool>>();
            RedisManager.GetCommonRedisClient(RedisDataType.Firewall).KeyDelete(firewallName);
            Func<Task<bool>> func = () =>
            {
                var firewall = RedisManager.GetFirewallClient(firewallName, limit, TimeSpan.FromSeconds(60));
                return firewall.HitAsync();
            };
            for (var i = 0; i < taskCount; i++)
            {
                tasks.Add(func());
            }
            await Task.WhenAll(tasks);

            Assert.Equal(limit, tasks.Count(_ => _.Result));
        }

        [Fact]
        public void RedisLockTest()
        {
            using (var client = RedisManager.GetRedLockClient("redLockTest"))
            {
                Assert.True(client.TryLock());
                using (var client1 = RedisManager.GetRedLockClient("redLockTest"))
                {
                    Assert.False(client1.TryLock());
                    Assert.False(client1.Release());
                }
                Assert.True(client.Release());
                using (var client1 = RedisManager.GetRedLockClient("redLockTest"))
                {
                    Assert.True(client1.TryLock(TimeSpan.FromSeconds(10)));
                }
            }

            var key = Guid.NewGuid().ToString("N");
            using (var client = RedisManager.GetRedLockClient(key))
            {
                Assert.True(client.TryLock(TimeSpan.FromSeconds(20)));
            }

            using (var client = RedisManager.GetRedLockClient(key))
            {
                Assert.True(client.TryLock());
                Assert.True(client.Release());
            }

            using (var client = RedisManager.GetRedLockClient(key, 3))
            {
                Assert.True(client.TryLock(TimeSpan.FromSeconds(1)));
                Assert.True(client.TryLock());
                Assert.False(client.TryLock());
            }
        }
    }
}
