using System;
using WeihanLi.Common;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class DependencyInjectionTest
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionTest(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [Fact]
        public void Test()
        {
            var cacheClient = _serviceProvider.ResolveService<ICacheClient>();
            Assert.NotNull(cacheClient);

            var key = Guid.NewGuid().ToString("N");
            Assert.Equal("abcaaa", cacheClient.GetOrSet(key, () => "abcaaa", TimeSpan.FromMinutes(10)));
            Assert.True(cacheClient.Remove(key));

            var hashClient = _serviceProvider.ResolveService<IHashClient>();
            Assert.NotNull(hashClient);

            var pubSubClient = _serviceProvider.ResolveService<IPubSubClient>();
            Assert.NotNull(pubSubClient);
        }
    }
}
