// ReSharper disable once CheckNamespace
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    public interface IHashClient
    {
    }

    internal class HashClient : BaseRedisClient, IHashClient
    {
        public HashClient() : base(LogHelper.GetLogHelper<HashClient>(), new RedisWrapper("Hash/"))
        {
        }
    }
}
