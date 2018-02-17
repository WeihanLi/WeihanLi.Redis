// ReSharper disable once CheckNamespace
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    public interface ISetClient
    {
    }

    internal class SetClient : BaseRedisClient, ISetClient
    {
        public SetClient() : base(LogHelper.GetLogHelper<SetClient>(), new RedisWrapper("Set/"))
        {
        }
    }
}
