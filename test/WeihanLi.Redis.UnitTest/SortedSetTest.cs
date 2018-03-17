using StackExchange.Redis;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class SortedSetTest : BaseUnitTest
    {
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
