using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class SetClient : BaseRedisClient, ISetClient
    {
        public SetClient() : base(LogHelper.GetLogHelper<SetClient>(), new RedisWrapper("Set"))
        {
        }
    }
}
