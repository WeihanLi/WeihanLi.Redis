using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class PubSubTest : BaseUnitTest
    {
        [Fact]
        public async Task MainTest()
        {
            var channelName = "testChannel";

            var counter = 0;
            RedisManager.PubSubClient.Subscribe(channelName, _ => Interlocked.Increment(ref counter));

            RedisManager.PubSubClient.Publish(channelName, new PubSubMessageModel { SubscribeType = "AA", SubscribeMessage = "Hahaha" });
            await Task.Delay(10000);
            Assert.Equal(1, counter);

            RedisManager.PubSubClient.Unsubscribe(channelName);
            await Task.Delay(3000);
            RedisManager.PubSubClient.Publish(channelName, new PubSubMessageModel { SubscribeType = "AA", SubscribeMessage = "Hahaha" });
            await Task.Delay(10000);
            Assert.Equal(1, counter);

            await Task.Delay(3000);
            RedisManager.PubSubClient.Subscribe(channelName, _ => Interlocked.Increment(ref counter));
            RedisManager.PubSubClient.Publish(channelName, new PubSubMessageModel { SubscribeType = "AA", SubscribeMessage = "Hahaha" });
            await Task.Delay(10000);
            Assert.Equal(2, counter);
            RedisManager.PubSubClient.Unsubscribe(channelName);
        }
    }
}
