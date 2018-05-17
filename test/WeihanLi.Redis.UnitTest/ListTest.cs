using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class ListTest : BaseUnitTest
    {
        [Fact]
        public void MainTest()
        {
            var testListKey = "testList";
            var commonClient = RedisManager.GetCommonRedisClient(RedisDataType.List);
            Assert.False(commonClient.KeyExists(testListKey));
            var listClient = RedisManager.GetListClient<string>(testListKey);
            Assert.False(commonClient.KeyExists(testListKey));
            Assert.Equal(0, listClient.Count());
            listClient.Push(new[] { "Michael", "Kangkang", "Kangkang" });
            Assert.True(commonClient.KeyExists(testListKey));
            Assert.Equal(3, listClient.Count());
            Assert.Equal("Kangkang", listClient.Pop());
            Assert.Equal(2, listClient.Count());
            Assert.Equal("Michael", listClient.LeftPop());
            Assert.Equal(1, listClient.Count());

            Assert.True(commonClient.KeyDelete(testListKey));
            Assert.False(commonClient.KeyExists(testListKey));
        }
    }
}
