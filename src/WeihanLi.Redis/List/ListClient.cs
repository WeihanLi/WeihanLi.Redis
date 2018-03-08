using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class ListClient<T> : BaseRedisClient, IListClient<T>
    {
        private readonly string _realKey;

        public ListClient(string keyName) : base(LogHelper.GetLogHelper<ListClient<T>>(), new RedisWrapper("List"))
        {
            _realKey = $"{Wrapper.KeyPrefix}/{keyName}";
        }

        public long Count(CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListLength(_realKey, flags);

        public Task<long> CountAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListLengthAsync(_realKey, flags);

        public T Get(long index, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.ListGetByIndex(_realKey, index, flags));

        public Task<T> GetAsync(long index, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.ListGetByIndexAsync(_realKey, index, flags));

        public long InsertAfter(T pivot, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListInsertAfter(_realKey, Wrapper.Wrap(pivot), Wrapper.Wrap(value), flags);

        public Task<long> InsertAfterAsync(T pivot, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListInsertAfterAsync(_realKey, Wrapper.Wrap(pivot), Wrapper.Wrap(value), flags);

        public long InsertBefore(T pivot, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListInsertBefore(_realKey, Wrapper.Wrap(pivot), Wrapper.Wrap(value), flags);

        public Task<long> InsertBeforeAsync(T pivot, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListInsertBeforeAsync(_realKey, Wrapper.Wrap(pivot), Wrapper.Wrap(value), flags);

        public T LeftPop(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.ListLeftPop(_realKey, flags));

        public Task<T> LeftPopAsync(CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.ListLeftPopAsync(_realKey, flags));

        public long LeftPush(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListLeftPush(_realKey, Wrapper.Wrap(values), flags);

        public long LeftPush(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListLeftPush(_realKey, Wrapper.Wrap(value), when, flags);

        public Task<long> LeftPushAsync(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListLeftPushAsync(_realKey, Wrapper.Wrap(values), flags);

        public Task<long> LeftPushAsync(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListLeftPushAsync(_realKey, Wrapper.Wrap(value), when, flags);

        public T[] ListRange(long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.ListRange(_realKey, start, stop, flags));

        public Task<T[]> ListRangeAsync(long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.ListRangeAsync(_realKey, start, stop, flags));

        public T Pop(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.ListRightPop(_realKey, flags));

        public Task<T> PopAsync(CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.ListRightPopAsync(_realKey, flags));

        public long Push(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRightPush(_realKey, Wrapper.Wrap(value), when, flags);

        public long Push(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRightPush(_realKey, Wrapper.Wrap(values), flags);

        public Task<long> PushAsync(T value, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRightPushAsync(_realKey, Wrapper.Wrap(value), when, flags);

        public Task<long> PushAsync(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRightPushAsync(_realKey, Wrapper.Wrap(values), flags);

        public long Remove(T value, long count = 0, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRemove(_realKey, Wrapper.Wrap(value), count, flags);

        public Task<long> RemoveAsync(T value, long count = 0, CommandFlags flags = CommandFlags.None) => Wrapper.Database.ListRemoveAsync(_realKey, Wrapper.Wrap(value), count, flags);

        public T RightPopLeftPush(string destination, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(() => Wrapper.Database.ListRightPopLeftPush(_realKey, Wrapper.KeyPrefix + destination, flags));

        public Task<T> RightPopLeftPushAsync(string destination, CommandFlags flags = CommandFlags.None) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.ListRightPopLeftPushAsync(_realKey, Wrapper.KeyPrefix + destination, flags));

        public bool Set(long index, T value, CommandFlags flags = CommandFlags.None)
        {
            Wrapper.Database.ListSetByIndex(_realKey, index, Wrapper.Wrap(value), flags);
            return true;
        }

        public async Task<bool> SetAsync(long index, T value, CommandFlags flags = CommandFlags.None)
        {
            await Wrapper.Database.ListSetByIndexAsync(_realKey, index, Wrapper.Wrap(value), flags);
            return true;
        }

        public T[] Sort(long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(
            () => Wrapper.Database
                .Sort(_realKey, skip, take, order, sortType, Wrapper.Wrap(by),
                    get?.Select(_ => Wrapper.Wrap(_)).ToArray(), flags)
        );

        public long SortAndStore(string destination, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortAndStore($"{Wrapper.KeyPrefix}/{destination}", _realKey, skip, take, order, sortType,
            Wrapper.Wrap(by), get?.Select(_ => Wrapper.Wrap(_)).ToArray(), flags);

        public bool Trim(long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            Wrapper.Database.ListTrim(_realKey, start, stop, flags);
            return true;
        }

        public async Task<bool> TrimAsync(long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            await Wrapper.Database.ListTrimAsync(_realKey, start, stop, flags);
            return true;
        }
    }
}
