using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class StackClient<T> : BaseRedisClient, IStackClient<T>
    {
        private readonly string _realKey;

        public StackClient(string keyName, ILogger<StackClient<T>> logger) : base(logger, new RedisWrapper(RedisConstants.StackPrefix))
        {
            _realKey = Wrapper.GetRealKey(keyName);
        }

        public T Pop(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.ListRightPop(_realKey, flags));

        public Task<T> PopAsync(CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.ListRightPopAsync(_realKey, flags));

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
