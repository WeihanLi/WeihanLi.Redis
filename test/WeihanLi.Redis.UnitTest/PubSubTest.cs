using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class PubSubTest : BaseUnitTest
    {
        [Fact(Skip="PubSub")]
        public async Task MainTest()
        {
            var channelName = "testChannel";

            var counter = 0;
            await RedisManager.PubSubClient.SubscribeAsync(channelName, _ => Interlocked.Increment(ref counter));

            await RedisManager.PubSubClient.PublishAsync(channelName, new PubSubMessage { SubscribeType = "AA", SubscribeMessage = "Hahaha" });
            await Task.Delay(10000);
            Assert.Equal(1, counter);

            await RedisManager.PubSubClient.UnsubscribeAsync(channelName);
            await Task.Delay(3000);
            await RedisManager.PubSubClient.PublishAsync(channelName, new PubSubMessage { SubscribeType = "AA", SubscribeMessage = "Hahaha" });
            await Task.Delay(10000);
            Assert.Equal(1, counter);

            await Task.Delay(3000);
            await RedisManager.PubSubClient.SubscribeAsync(channelName, _ => Interlocked.Increment(ref counter));
            await RedisManager.PubSubClient.PublishAsync(channelName, new PubSubMessage { SubscribeType = "AA", SubscribeMessage = "Hahaha" });
            await Task.Delay(10000);
            Assert.Equal(2, counter);
            await RedisManager.PubSubClient.UnsubscribeAsync(channelName);
        }
    }
}
