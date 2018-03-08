using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class RankClient : BaseRedisClient, IRankClient
    {
        public RankClient() : base(LogHelper.GetLogHelper<RankClient>(), new RedisWrapper("SortedSet/Rank"))
        {
        }
    }
}
