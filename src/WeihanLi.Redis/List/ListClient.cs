using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Extensions;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class ListClient<T> : BaseRedisClient, IListClient<T>
    {
        private readonly string _realKey;

        public ListClient(string keyName, ILogger<ListClient<T>> logger) : base(logger, new RedisWrapper(RedisConstants.ListPrefix))
        {
            _realKey = Wrapper.GetRealKey(keyName);
        }

        public long Count(CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListLength(_realKey, flags);

        public Task<long> CountAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListLengthAsync(_realKey, flags);

        public T Get(long index, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.Value.ListGetByIndex(_realKey, index, flags));

        public Task<T> GetAsync(long index, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.Value.ListGetByIndexAsync(_realKey, index, flags));

        public long InsertAfter(T pivot, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListInsertAfter(_realKey, Wrapper.Wrap(pivot), Wrapper.Wrap(value), flags);

        public Task<long> InsertAfterAsync(T pivot, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListInsertAfterAsync(_realKey, Wrapper.Wrap(pivot), Wrapper.Wrap(value), flags);

        public long InsertBefore(T pivot, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListInsertBefore(_realKey, Wrapper.Wrap(pivot), Wrapper.Wrap(value), flags);

        public Task<long> InsertBeforeAsync(T pivot, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListInsertBeforeAsync(_realKey, Wrapper.Wrap(pivot), Wrapper.Wrap(value), flags);

        public T LeftPop(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.Value.ListLeftPop(_realKey, flags));

        public Task<T> LeftPopAsync(CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.Value.ListLeftPopAsync(_realKey, flags));

        public long LeftPush(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListLeftPush(_realKey, Wrapper.Wrap(values), flags);

        public long LeftPush(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListLeftPush(_realKey, Wrapper.Wrap(value), when, flags);

        public Task<long> LeftPushAsync(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListLeftPushAsync(_realKey, Wrapper.Wrap(values), flags);

        public Task<long> LeftPushAsync(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListLeftPushAsync(_realKey, Wrapper.Wrap(value), when, flags);

        public T[] ListRange(long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.Value.ListRange(_realKey, start, stop, flags));

        public Task<T[]> ListRangeAsync(long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.Value.ListRangeAsync(_realKey, start, stop, flags));

        public T Pop(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.Value.ListRightPop(_realKey, flags));

        public Task<T> PopAsync(CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.Value.ListRightPopAsync(_realKey, flags));

        public long Push(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRightPush(_realKey, Wrapper.Wrap(value), when, flags);

        public long Push(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRightPush(_realKey, Wrapper.Wrap(values), flags);

        public Task<long> PushAsync(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRightPushAsync(_realKey, Wrapper.Wrap(value), when, flags);

        public Task<long> PushAsync(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRightPushAsync(_realKey, Wrapper.Wrap(values), flags);

        public long Remove(T value, long count = 0, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRemove(_realKey, Wrapper.Wrap(value), count, flags);

        public Task<long> RemoveAsync(T value, long count = 0, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.ListRemoveAsync(_realKey, Wrapper.Wrap(value), count, flags);

        public T RightPopLeftPush(string destination, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.Value.ListRightPopLeftPush(_realKey, Wrapper.KeyPrefix + destination, flags));

        public Task<T> RightPopLeftPushAsync(string destination, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.Value.ListRightPopLeftPushAsync(_realKey, Wrapper.KeyPrefix + destination, flags));

        public bool Set(long index, T value, CommandFlags flags = CommandFlags.None)
        {
            Wrapper.Database.Value.ListSetByIndex(_realKey, index, Wrapper.Wrap(value), flags);
            return true;
        }

        public async Task<bool> SetAsync(long index, T value, CommandFlags flags = CommandFlags.None)
        {
            await Wrapper.Database.Value.ListSetByIndexAsync(_realKey, index, Wrapper.Wrap(value), flags);
            return true;
        }

        public T[] Sort(long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(
            () => Wrapper.Database.Value
                .Sort(_realKey, skip, take, order, sortType, Wrapper.Wrap(by),
                    get?.Select(_ => Wrapper.Wrap(_)).ToArray(), flags)
        );

        public long SortAndStore(string destination, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortAndStore($"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}", _realKey, skip, take, order, sortType,
            Wrapper.Wrap(by), get?.Select(_ => Wrapper.Wrap(_)).ToArray(), flags);

        public bool Trim(long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            Wrapper.Database.Value.ListTrim(_realKey, start, stop, flags);
            return true;
        }

        public async Task<bool> TrimAsync(long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            await Wrapper.Database.Value.ListTrimAsync(_realKey, start, stop, flags);
            return true;
        }
    }
}
