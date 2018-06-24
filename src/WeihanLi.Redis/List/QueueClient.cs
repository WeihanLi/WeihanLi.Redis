using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Redis.Internals;

namespace WeihanLi.Redis.List
{
    internal class QueueClient<T> : BaseRedisClient, IQueueClient<T>
    {
        private readonly string _realKey;

        public QueueClient(string keyName) : base(LogHelper.GetLogHelper<QueueClient<T>>(), new RedisWrapper(RedisConstants.QueuePrefix))
        {
            _realKey = Wrapper.GetRealKey(keyName);
        }

        public T Pop(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.ListLeftPop(_realKey, flags));

        public Task<T> PopAsync(CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.ListLeftPopAsync(_realKey, flags));

        public long Push(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRightPush(_realKey, Wrapper.Wrap(value), when, flags);

        public long Push(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRightPush(_realKey, Wrapper.Wrap(values), flags);

        public Task<long> PushAsync(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRightPushAsync(_realKey, Wrapper.Wrap(value), when, flags);

        public Task<long> PushAsync(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRightPushAsync(_realKey, Wrapper.Wrap(values), flags);

        public long Length(CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListLength(_realKey, flags);

        public Task<long> LengthAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListLengthAsync(_realKey, flags);

        public T this[long index]
        {
            get => Wrapper.Unwrap<T>(Wrapper.Database.ListGetByIndex(_realKey, index));
            set => Wrapper.Database.ListSetByIndex(_realKey, index, Wrapper.Wrap(value));
        }
    }
}
