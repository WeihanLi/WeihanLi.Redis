using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class ListTest
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

        [Fact]
        public void QueueTest()
        {
            var testKey = "testQueue";
            var commonClient = RedisManager.GetCommonRedisClient(RedisDataType.Queue);
            try
            {
                Assert.False(commonClient.KeyExists(testKey));
                var queueClient = RedisManager.GetQueueClient<string>(testKey);
                Assert.Equal(0, queueClient.Length());
                Assert.Null(queueClient.Pop());
                var result = queueClient.Push(new[] { "xiaoming", "xiaohong", "xiaowangba" });
                Assert.Equal(3, result);
                result = queueClient.Push("xiaogang");
                Assert.Equal(4, result);
                Assert.Equal(4, queueClient.Length());
                Assert.Equal("xiaoming", queueClient[0]);
                Assert.Equal("xiaoming", queueClient.Pop());
                Assert.Equal(3, queueClient.Length());
                Assert.True(commonClient.KeyExists(testKey));
            }
            finally
            {
                Assert.True(commonClient.KeyDelete(testKey));
                Assert.False(commonClient.KeyExists(testKey));
            }
        }

        [Fact]
        public void StackTest()
        {
            var testKey = "testStack";
            var commonClient = RedisManager.GetCommonRedisClient(RedisDataType.Stack);
            Assert.False(commonClient.KeyExists(testKey));
            var client = RedisManager.GetStackClient<string>(testKey);
            Assert.Equal(0, client.Length());
            Assert.Null(client.Pop());
            var result = client.Push(new[] { "xiaoming", "xiaohong", "xiaowangba" });
            Assert.Equal(3, result);
            result = client.Push("xiaogang");
            Assert.Equal(4, result);
            Assert.Equal(4, client.Length());
            Assert.Equal("xiaoming", client[0]);
            Assert.Equal("xiaogang", client.Pop());
            Assert.Equal(3, client.Length());
            Assert.True(commonClient.KeyExists(testKey));
            Assert.True(commonClient.KeyDelete(testKey));
            Assert.False(commonClient.KeyExists(testKey));
        }
    }
}
