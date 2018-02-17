// ReSharper disable once CheckNamespace
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    public interface IListClient
    {
    }

    internal class ListClient : BaseRedisClient, IListClient
    {
        public ListClient() : base(LogHelper.GetLogHelper<ListClient>(), new RedisWrapper("List/"))
        {
        }
    }
}
