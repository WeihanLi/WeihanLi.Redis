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
            Assert.Equal("abcaaa", cacheClient.GetOrSet("abc", () => "abcaaa", TimeSpan.FromSeconds(10)));

            var hashClient = DependencyResolver.Current.ResolveService<IHashClient>();
            Assert.NotNull(hashClient);
            var pubsubClient = DependencyResolver.Current.ResolveService<IPubSubClient>();
            Assert.NotNull(pubsubClient);
        }
    }
}
