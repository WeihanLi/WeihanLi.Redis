using System;
using WeihanLi.Common;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class DependencyInjectionTest : BaseUnitTest
    {
        [Fact]
        public void Test()
        {
            var cacheClient = DependencyResolver.Current.ResolveService<ICacheClient>();
            Assert.NotNull(cacheClient);
            var key = Guid.NewGuid().ToString("N");
            Assert.Equal("abcaaa", cacheClient.GetOrSet(key, () => "abcaaa", TimeSpan.FromMinutes(10)));
            cacheClient.Remove(key);
            var hashClient = DependencyResolver.Current.ResolveService<IHashClient>();
            Assert.NotNull(hashClient);
            var pubsubClient = DependencyResolver.Current.ResolveService<IPubSubClient>();
            Assert.NotNull(pubsubClient);
        }
    }
}
