using WeihanLi.Common.Helpers;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class RankClient : BaseRedisClient, IRankClient
    {
        public RankClient() : base(LogHelper.GetLogHelper<RankClient>(), new RedisWrapper(RedisConstants.RankPrefix))
        {
        }
    }
}
