// ReSharper disable once CheckNamespace
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    public interface IRankClient
    {
    }

    internal class RankClient : BaseRedisClient, IRankClient
    {
        public RankClient() : base(LogHelper.GetLogHelper<RankClient>(), new RedisWrapper("SortedSet/Rank/"))
        {
        }
    }
}
