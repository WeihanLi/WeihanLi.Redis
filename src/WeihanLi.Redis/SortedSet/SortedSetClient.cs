using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class SortedSetClient : BaseRedisClient, ISortedSetClient
    {
        public SortedSetClient() : base(LogHelper.GetLogHelper<SortedSetClient>(), new RedisWrapper("SortedSet"))
        {
        }
    }
}
