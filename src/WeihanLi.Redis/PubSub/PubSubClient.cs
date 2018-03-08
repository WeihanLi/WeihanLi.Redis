using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    /// <summary>
    /// Redis订阅消息Model
    /// </summary>
    internal class PubSubMessageModel : IPubSubMessage
    {
        /// <summary>
        /// channel
        /// </summary>
        public string Channnel { get; set; }

        /// <summary>
        /// 订阅类型
        /// </summary>
        public string SubscribeType { get; set; }

        /// <summary>
        /// 订阅消息
        /// </summary>
        public string SubscribeMessage { get; set; }
    }

    internal class PubSubClient : BaseRedisClient, IPubSubClient
    {
        public PubSubClient() : base(LogHelper.GetLogHelper<PubSubClient>(), new RedisWrapper("PubSub"))
        {
        }

        public bool IsConnected(string channelName) => channelName.IsNullOrWhiteSpace() ? Wrapper.Subscriber.IsConnected() : Wrapper.Subscriber.IsConnected($"{Wrapper.ChannelPrefix}/{channelName}");

        public long Publish(string channelName, string message, CommandFlags flags = CommandFlags.None) => Wrapper.Subscriber.Publish($"{Wrapper.ChannelPrefix}/{channelName}", Wrapper.Wrap(message), flags);

        public Task<long> PublishAsync(string channelName, string message, CommandFlags flags = CommandFlags.None) => Wrapper.Subscriber.PublishAsync($"{Wrapper.ChannelPrefix}/{channelName}", Wrapper.Wrap(message), flags);

        public void Subscribe(string channelName, string type, Action<IPubSubMessage> action, CommandFlags flag = CommandFlags.None) => Wrapper.Subscriber.Subscribe($"{Wrapper.ChannelPrefix}/{channelName}", (c, v) => action(Wrapper.Unwrap<PubSubMessageModel>(v)));

        public Task SubscribeAsync(string channelName, string type, Action<IPubSubMessage> action, CommandFlags flag = CommandFlags.None) => Wrapper.Subscriber.SubscribeAsync($"{Wrapper.ChannelPrefix}/{channelName}", (c, v) => action(Wrapper.Unwrap<PubSubMessageModel>(v)));

        public void Unsubscribe(string channelName, Action<IPubSubMessage> handler = null, CommandFlags flags = CommandFlags.None)
        {
            if (null == handler)
            {
                Wrapper.Subscriber.Unsubscribe($"{Wrapper.ChannelPrefix}/{channelName}", flags: flags);
            }
            else
            {
                Wrapper.Subscriber.Unsubscribe($"{Wrapper.ChannelPrefix}/{channelName}", (channel, value) => handler(Wrapper.Unwrap<PubSubMessageModel>(value)), flags);
            }
        }

        public async Task UnsubscribeAsync(string channelName, Action<IPubSubMessage> handler = null, CommandFlags flags = CommandFlags.None)
        {
            if (null == handler)
            {
                await Wrapper.Subscriber.UnsubscribeAsync($"{Wrapper.ChannelPrefix}/{channelName}", flags: flags);
            }
            else
            {
                await Wrapper.Subscriber.UnsubscribeAsync($"{Wrapper.ChannelPrefix}/{channelName}", (channel, value) => handler(Wrapper.Unwrap<PubSubMessageModel>(value)), flags);
            }
        }

        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None) => Wrapper.Subscriber.UnsubscribeAll(flags);

        public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Subscriber.UnsubscribeAllAsync(flags);
    }
}
