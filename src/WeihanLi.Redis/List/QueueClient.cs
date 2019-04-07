using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Redis.Internals;

namespace WeihanLi.Redis.List
{
    internal class QueueClient<T> : BaseRedisClient, IQueueClient<T>
    {
        private readonly string _realKey;

        public QueueClient(string keyName, ILogger<QueueClient<T>> logger) : base(logger, new RedisWrapper(RedisConstants.QueuePrefix))
        {
            _realKey = Wrapper.GetRealKey(keyName);
        }

        public T Pop(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.Value.ListLeftPop(_realKey, flags));

        public Task<T> PopAsync(CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.Value.ListLeftPopAsync(_realKey, flags));

        public long Push(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRightPush(_realKey, Wrapper.Wrap(value), when, flags);

        public long Push(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRightPush(_realKey, Wrapper.Wrap(values), flags);

        public Task<long> PushAsync(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRightPushAsync(_realKey, Wrapper.Wrap(value), when, flags);

        public Task<long> PushAsync(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRightPushAsync(_realKey, Wrapper.Wrap(values), flags);

        public long Length(CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListLength(_realKey, flags);

        public Task<long> LengthAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListLengthAsync(_realKey, flags);

        public T this[long index]
        {
            get => Wrapper.Unwrap<T>(Wrapper.Database.Value.ListGetByIndex(_realKey, index));
            set => Wrapper.Database.Value.ListSetByIndex(_realKey, index, Wrapper.Wrap(value));
        }
    }
}
