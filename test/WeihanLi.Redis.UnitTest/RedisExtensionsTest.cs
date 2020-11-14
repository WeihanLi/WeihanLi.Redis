using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using WeihanLi.Common;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class RedisExtensionsTest
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

        [Fact]
        public void StringCompareAndDeleteTest()
        {
            var key = "test:String:cad";
            var redis = DependencyResolver.Current
                .GetRequiredService<IConnectionMultiplexer>()
                .GetDatabase();
            redis.StringSet(key, 1);

            // delete if now is 2
            Assert.False(redis.StringCompareAndDelete(key, 3));
            Assert.True(redis.KeyExists(key));

            // delete if now is 1
            Assert.True(redis.StringCompareAndDelete(key, 1));
            Assert.False(redis.KeyExists(key));
        }

        [Fact]
        public void HashCompareAndDeleteTest()
        {
            var key = "test:Hash:cad";
            var field = "testField";

            var redis = DependencyResolver.Current
                .GetRequiredService<IConnectionMultiplexer>()
                .GetDatabase();
            redis.HashSet(key, field, 1);

            // delete field if now is 2
            Assert.False(redis.HashCompareAndDelete(key, field, 2));
            Assert.True(redis.HashExists(key, field));

            // delete field if now is 1
            Assert.True(redis.HashCompareAndDelete(key, field, 1));
            Assert.False(redis.HashExists(key, field));
        }
    }
}
