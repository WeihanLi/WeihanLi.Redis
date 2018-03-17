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
        private static string GetRealChannelName(string channelName) => $"{RedisManager.RedisConfiguration.ChannelPrefix}{RedisManager.RedisConfiguration.KeySeparator}{channelName}";

        public PubSubClient() : base(LogHelper.GetLogHelper<PubSubClient>(), new RedisWrapper("PubSub"))
        {
        }

        public bool IsConnected(string channelName = null) => channelName.IsNullOrWhiteSpace() ? Wrapper.Subscriber.IsConnected() : Wrapper.Subscriber.IsConnected(GetRealChannelName(channelName));

        public long Publish(string channelName, string message, CommandFlags flags = CommandFlags.None) => Wrapper.Subscriber.Publish(GetRealChannelName(channelName), Wrapper.Wrap(message), flags);

        public Task<long> PublishAsync(string channelName, string message, CommandFlags flags = CommandFlags.None) => Wrapper.Subscriber.PublishAsync(GetRealChannelName(channelName), Wrapper.Wrap(message), flags);

        public void Subscribe(string channelName, string type, Action<IPubSubMessage> action, CommandFlags flag = CommandFlags.None) => Wrapper.Subscriber.Subscribe(GetRealChannelName(channelName), (c, v) => action(Wrapper.Unwrap<PubSubMessageModel>(v)));

        public Task SubscribeAsync(string channelName, string type, Action<IPubSubMessage> action, CommandFlags flag = CommandFlags.None) => Wrapper.Subscriber.SubscribeAsync(GetRealChannelName(channelName), (c, v) => action(Wrapper.Unwrap<PubSubMessageModel>(v)));

        public void Unsubscribe(string channelName, Action<IPubSubMessage> handler = null, CommandFlags flags = CommandFlags.None)
        {
            if (null == handler)
            {
                Wrapper.Subscriber.Unsubscribe(GetRealChannelName(channelName), flags: flags);
            }
            else
            {
                Wrapper.Subscriber.Unsubscribe(GetRealChannelName(channelName), (channel, value) => handler(Wrapper.Unwrap<PubSubMessageModel>(value)), flags);
            }
        }

        public async Task UnsubscribeAsync(string channelName, Action<IPubSubMessage> handler = null, CommandFlags flags = CommandFlags.None)
        {
            if (null == handler)
            {
                await Wrapper.Subscriber.UnsubscribeAsync(GetRealChannelName(channelName), flags: flags);
            }
            else
            {
                await Wrapper.Subscriber.UnsubscribeAsync(GetRealChannelName(channelName), (channel, value) => handler(Wrapper.Unwrap<PubSubMessageModel>(value)), flags);
            }
        }

        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None) => Wrapper.Subscriber.UnsubscribeAll(flags);

        public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Subscriber.UnsubscribeAllAsync(flags);
    }
}
