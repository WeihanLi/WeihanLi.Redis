using StackExchange.Redis;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class SortedSetTest
    {
        [Fact]
        public void MainTest()
        {
            var testSortedSetKey = "testSortedSet";
            var sortedSetClient = RedisManager.GetSortedSetClient<string>(testSortedSetKey);
            Assert.Equal(0, sortedSetClient.Length());
            sortedSetClient.Add("Michael", 10);
            sortedSetClient.Add("Kangkang", 20);
            sortedSetClient.Add("Kangkang", 20);
            sortedSetClient.Add("Jane", 40);
            Assert.Equal(3, sortedSetClient.Length());
            Assert.Equal("Jane", sortedSetClient.RangeByRank()[2]);
            Assert.Equal("Michael", sortedSetClient.RangeByScore()[0]);
            Assert.True(sortedSetClient.Remove("Michael"));
            Assert.Equal(2, sortedSetClient.Length());
            var commonClient = RedisManager.GetCommonRedisClient(RedisDataType.SortedSet);
            Assert.True(commonClient.KeyDelete(testSortedSetKey));
        }

        [Fact]
        public void RankTest()
        {
            var rankClient = RedisManager.GetRankClient<string>("testRank");
            Assert.Equal(0, rankClient.Length());
            rankClient.Add("xiaoming", 100);
            rankClient.Add("xiaohong", 95);
            rankClient.Add("xiaowang", 96);
            Assert.Equal(3, rankClient.Length());
            Assert.Equal(100, rankClient.Score("xiaoming"));
            var rank = rankClient.RangeByScore();
            Assert.Equal("xiaohong", rank[0]);
            rank = rankClient.RangeByScore(order: Order.Descending);
            Assert.Equal("xiaoming", rank[0]);
            var common = RedisManager.GetCommonRedisClient(RedisDataType.Rank);
            Assert.True(common.KeyDelete("testRank"));
        }
    }
}
