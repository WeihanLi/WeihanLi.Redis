using System;
using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface IPubSubClient : IRedisClient
    {
        /// <summary>
        /// Indicates whether the instance can communicate with the server;
        /// if a channel is specified, the existing subscription map is queried to
        /// resolve the server responsible for that subscription - otherwise the
        /// server is chosen aribtraily from the masters.
        /// </summary>
        bool IsConnected(string channelName = null);

        /// <summary>Posts a message to the given channel.</summary>
        /// <returns>the number of clients that received the message.</returns>
        /// <remarks>http://redis.io/commands/publish</remarks>
        long Publish(string channelName, string message, CommandFlags flags = CommandFlags.None);

        /// <summary>Posts a message to the given channel.</summary>
        /// <returns>the number of clients that received the message.</returns>
        /// <remarks>http://redis.io/commands/publish</remarks>
        Task<long> PublishAsync(string channelName, string message, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Subscribe to perform some operation when a change to the preferred/active node is broadcast.
        /// </summary>
        /// <remarks>http://redis.io/commands/subscribe</remarks>
        /// <remarks>http://redis.io/commands/psubscribe</remarks>
        void Subscribe(string channelName, string type, Action<IPubSubMessage> action, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Subscribe to perform some operation when a change to the preferred/active node is broadcast.
        /// </summary>
        /// <remarks>http://redis.io/commands/subscribe</remarks>
        /// <remarks>http://redis.io/commands/psubscribe</remarks>
        Task SubscribeAsync(string channelName, string type, Action<IPubSubMessage> action, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Unsubscribe from a specified message channel; note; if no handler is specified, the subscription is cancelled regardless
        /// of the subscribers; if a handler is specified, the subscription is only cancelled if this handler is the
        /// last handler remaining against the channel
        /// </summary>
        /// <remarks>http://redis.io/commands/unsubscribe</remarks>
        /// <remarks>http://redis.io/commands/punsubscribe</remarks>
        void Unsubscribe(string channelName, Action<IPubSubMessage> handler = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Unsubscribe from a specified message channel; note; if no handler is specified, the subscription is cancelled regardless
        /// of the subscribers; if a handler is specified, the subscription is only cancelled if this handler is the
        /// last handler remaining against the channel
        /// </summary>
        /// <remarks>http://redis.io/commands/unsubscribe</remarks>
        /// <remarks>http://redis.io/commands/punsubscribe</remarks>
        Task UnsubscribeAsync(string channelName, Action<IPubSubMessage> handler = null, CommandFlags flags = CommandFlags.None);

        /// <summary>Unsubscribe all subscriptions on this instance</summary>
        /// <remarks>http://redis.io/commands/unsubscribe</remarks>
        /// <remarks>http://redis.io/commands/punsubscribe</remarks>
        void UnsubscribeAll(CommandFlags flags = CommandFlags.None);

        /// <summary>Unsubscribe all subscriptions on this instance</summary>
        /// <remarks>http://redis.io/commands/unsubscribe</remarks>
        /// <remarks>http://redis.io/commands/punsubscribe</remarks>
        Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None);
    }

    public interface IPubSubMessage
    {
        /// <summary>
        /// channel
        /// </summary>
        string Channnel { get; set; }

        /// <summary>
        /// 订阅类型
        /// </summary>
        string SubscribeType { get; set; }

        /// <summary>
        /// 订阅消息
        /// </summary>
        string SubscribeMessage { get; set; }
    }
}
