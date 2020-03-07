using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using WeihanLi.Common;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class RedisExtensionsTest : BaseUnitTest
    {
        [Fact]
        public void StringCompareAndExchangeTest()
        {
            var key = "test:String:cas";
            var redis = DependencyResolver.Current
                .GetRequiredService<IConnectionMultiplexer>()
                .GetDatabase();
            redis.StringSet(key, 1);

            // set to 3 if now is 2
            Assert.False(redis.StringCompareAndExchange(key, 3, 2));
            Assert.Equal(1, redis.StringGet(key));

            // set to 4 if now is 1
            Assert.True(redis.StringCompareAndExchange(key, 4, 1));
            Assert.Equal(4, redis.StringGet(key));

            redis.KeyDelete(key);
        }

        [Fact]
        public void HashCompareAndExchangeTest()
        {
            var key = "test:Hash:cas";
            var field = "testField";

            var redis = DependencyResolver.Current
                .GetRequiredService<IConnectionMultiplexer>()
                .GetDatabase();
            redis.HashSet(key, field, 1);

            // set to 3 if now is 2
            Assert.False(redis.HashCompareAndExchange(key, field, 3, 2));
            Assert.Equal(1, redis.HashGet(key, field));

            // set to 4 if now is 1
            Assert.True(redis.HashCompareAndExchange(key, field, 4, 1));
            Assert.Equal(4, redis.HashGet(key, field));

            redis.KeyDelete(key);
        }
    }
}
