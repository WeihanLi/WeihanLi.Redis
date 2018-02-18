using System;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class HashTest : BaseUnitTest
    {
        [Fact]
        public void HashCacheTest()
        {
            var key = "hashTest";
            var fieldName = "testField1";
            var value = "Hello WeihanLi.Redis";
            var hashClient = RedisManager.GetHashClient();
            var result = hashClient.Set(key, fieldName, value);
            Assert.True(hashClient.Expire(key, TimeSpan.FromSeconds(10)));
            Assert.True(hashClient.Exists(key, fieldName));
            Assert.Equal(value, hashClient.Get(key, fieldName));
            Assert.True(hashClient.Remove(key, fieldName));
            Assert.False(hashClient.Exists(key, fieldName));
        }
    }
}
