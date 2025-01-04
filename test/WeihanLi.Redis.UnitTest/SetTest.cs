using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class SetTest
    {
        [Fact]
        public void MainTest()
        {
            var testSetKey = "testSet";
            var setClient = RedisManager.GetSetClient<string>(testSetKey);
            Assert.Equal(0, setClient.Length());
            setClient.Add(new[] { "Michael", "Kangkang", "Kangkang" });
            Assert.Equal(2, setClient.Length());
            Assert.True(setClient.Remove("Kangkang"));
            Assert.Equal(1, setClient.Length());
            var commonClient = RedisManager.GetCommonRedisClient(RedisDataType.Set);
            Assert.True(commonClient.KeyDelete(testSetKey));
        }
    }
}
