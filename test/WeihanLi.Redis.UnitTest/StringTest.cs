using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class StringTest
    {
        [Fact]
        public void StringCacheTest()
        {
            var key = "test111";
            var value = "Hello WeihanLi.Redis";
            var cacheClient = RedisManager.CacheClient;
            Assert.False(cacheClient.Exists(key));
            Assert.Null(cacheClient.Get(key));
            Assert.True(cacheClient.Set(key, 1, TimeSpan.FromSeconds(10)));
            Assert.True(cacheClient.Set(key, value));
            Assert.True(cacheClient.Exists(key));
            Assert.Equal(value, cacheClient.Get(key));
            Assert.True(cacheClient.Remove(key));
            Assert.False(cacheClient.Exists(key));
            Assert.Null(cacheClient.GetOrSet("testGetOrSet", () => (string)null, TimeSpan.FromSeconds(20)));
        }

        [Fact]
        public async Task StringCacheAsyncTest()
        {
            var key = "test111Async";
            var value = "Hello WeihanLi.Redis";
            var cacheClient = RedisManager.CacheClient;
            Assert.False(await cacheClient.ExistsAsync(key));
            Assert.Null(await cacheClient.GetAsync(key));
            Assert.True(await cacheClient.SetAsync(key, 1, TimeSpan.FromSeconds(10)));
            Assert.True(await cacheClient.SetAsync(key, value));
            Assert.True(await cacheClient.ExistsAsync(key));
            Assert.Equal(value, await cacheClient.GetAsync(key));
            Assert.True(await cacheClient.RemoveAsync(key));
            Assert.False(await cacheClient.ExistsAsync(key));
            Assert.Null(await cacheClient.GetOrSetAsync("testGetOrSetAsync", () => Task.FromResult((string)null), TimeSpan.FromSeconds(20)));
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
        [InlineData(50)]
        public async Task CounterConcurrentTest(int taskCount)
        {
            var counterName = $"concurrentCounterTest_{taskCount}";

            await RedisManager.GetCommonRedisClient(RedisDataType.Counter).KeyDeleteAsync(counterName);
            await Task.WhenAll(Enumerable.Range(1, taskCount)
                .Select(_ =>
                {
                    var counter = RedisManager.GetCounterClient(counterName, TimeSpan.FromMinutes(3));
                    return counter.IncreaseAsync();
                }));

            Assert.Equal(taskCount, RedisManager.GetCounterClient(counterName, TimeSpan.FromMinutes(3)).Count());
        }

        [Fact]
        public async Task StringCounterExpiryTest()
        {
            var counterName = nameof(StringCounterExpiryTest);
            var counter = RedisManager.GetCounterClient(counterName, TimeSpan.FromSeconds(5));
            await Task.WhenAll(Enumerable.Range(0, 5).Select(x => counter.IncreaseAsync()));
            Assert.Equal(5, counter.Count());
            await Task.Delay(TimeSpan.FromSeconds(10));
            Assert.Equal(1, await counter.IncreaseAsync());
            await Task.Delay(TimeSpan.FromSeconds(10));
            Assert.False(await RedisManager.GetCommonRedisClient(RedisDataType.Counter)
                .KeyExistsAsync(counterName)
            );
        }

        [Fact]
        public async Task StringFirewallTest()
        {
            var firewallName = "firewallTest";
            var firewallClient = RedisManager.GetFirewallClient(firewallName, TimeSpan.FromSeconds(3));

            Assert.True(await firewallClient.HitAsync());
            Assert.False(await firewallClient.HitAsync());
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.True(await firewallClient.HitAsync());

            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.False(await RedisManager.GetCommonRedisClient(RedisDataType.Firewall)
                .KeyExistsAsync(firewallName)
            );
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(3, 20)]
        [InlineData(1, 50)]
        public async Task StringFirewallConcurrentTest(int limit, int taskCount)
        {
            var firewallName = $"concurrentFirewallTest_{limit}_{taskCount}";
            await RedisManager.GetCommonRedisClient(RedisDataType.Firewall)
                .KeyDeleteAsync(firewallName);

            var results = await Task.WhenAll(Enumerable.Range(1, taskCount)
                .Select(_ =>
                {
                    var firewall = RedisManager.GetFirewallClient(firewallName, limit, TimeSpan.FromSeconds(60));
                    return firewall.HitAsync();
                }));

            Assert.Equal(limit, results.Count(_ => _));
        }

        [Fact]
        public async Task StringRateLimiterTest()
        {
            var rateLimiterName = "rateLimiterTest";
            var rateLimiterClient = RedisManager.GetRateLimiterClient(rateLimiterName, TimeSpan.FromSeconds(3));

            Assert.True(await rateLimiterClient.AcquireAsync());
            Assert.False(await rateLimiterClient.AcquireAsync());
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.True(await rateLimiterClient.AcquireAsync());
            Assert.True(await rateLimiterClient.ReleaseAsync());
            Assert.False(await rateLimiterClient.ReleaseAsync());

            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.False(await RedisManager.GetCommonRedisClient(RedisDataType.RateLimiter)
                .KeyExistsAsync(rateLimiterName)
            );
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(3, 20)]
        [InlineData(1, 50)]
        public async Task StringRateLimiterConcurrentTest(int limit, int taskCount)
        {
            var rateLimiterName = $"concurrentRateLimiterTest_{limit}_{taskCount}";
            var tasks = new List<Task<bool>>();
            await RedisManager.GetCommonRedisClient(RedisDataType.RateLimiter)
                .KeyDeleteAsync(rateLimiterName);
            Func<Task<bool>> func = () =>
            {
                var rateLimiter = RedisManager.GetRateLimiterClient(rateLimiterName, limit, TimeSpan.FromSeconds(60));
                return rateLimiter.AcquireAsync();
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
