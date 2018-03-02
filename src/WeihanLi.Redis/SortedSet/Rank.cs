using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface IRankClient
    {
    }

    internal class RankClient : BaseRedisClient, IRankClient
    {
        public RankClient() : base(LogHelper.GetLogHelper<RankClient>(), new RedisWrapper("SortedSet/Rank"))
        {
        }
    }
}
