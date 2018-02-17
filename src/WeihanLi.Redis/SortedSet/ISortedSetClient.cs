// ReSharper disable once CheckNamespace
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    public interface ISortedSetClient
    {
    }

    internal class SortedSetClient : BaseRedisClient, ISortedSetClient
    {
        public SortedSetClient() : base(LogHelper.GetLogHelper<SortedSetClient>(), new RedisWrapper("SortedSet/"))
        {
        }
    }
}
