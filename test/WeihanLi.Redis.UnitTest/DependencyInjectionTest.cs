using System;
using WeihanLi.Common;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class DependencyInjectionTest : BaseDependencyTest
    {
        [Fact]
        public void Test()
        {
            var cacheClient = DependencyResolver.Current.GetService<ICacheClient>();
            Assert.NotNull(cacheClient);
            Assert.Equal("abcaaa", cacheClient.GetOrSet("abc", () => "abcaaa", TimeSpan.FromSeconds(10)));

            var hashClient = DependencyResolver.Current.GetService<IHashClient>();
            Assert.NotNull(hashClient);
            var pubsubClient = DependencyResolver.Current.GetService<IPubSubClient>();
            Assert.NotNull(pubsubClient);
        }
    }
}
