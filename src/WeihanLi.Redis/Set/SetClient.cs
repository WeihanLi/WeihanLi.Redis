using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Extensions;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class SetClient<T> : BaseRedisClient, ISetClient<T>
    {
        private readonly string _keyName;

        public SetClient(string setName, ILogger<SetClient<T>> logger) : base(logger, new RedisWrapper(RedisConstants.SetPrefix))
        {
            _keyName = Wrapper.GetRealKey(setName);
        }

        public bool Add(T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetAdd(_keyName, Wrapper.Wrap(value), flags);

        public long Add(T[] values, CommandFlags flags = CommandFlags.None)
        => Wrapper.Database.Value.SetAdd(_keyName, Wrapper.Wrap(values), flags);

        public Task<bool> AddAsync(T value, CommandFlags flags = CommandFlags.None)
            => Wrapper.Database.Value.SetAddAsync(_keyName, Wrapper.Wrap(value), flags);

        public Task<long> AddAsync(T[] values, CommandFlags flags = CommandFlags.None)
            => Wrapper.Database.Value.SetAddAsync(_keyName, Wrapper.Wrap(values), flags);

        public T[] Combine(SetOperation operation, string another, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SetCombine(operation, _keyName, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{another}", flags));

        public T[] Combine(SetOperation operation, string[] keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(k => $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{k}").ToList();
            redisKeys.AddIfNotContains(_keyName);
            return Wrapper.Unwrap<T>(Wrapper.Database.Value.SetCombine(operation,
                redisKeys.Select(k => (RedisKey)k).ToArray()));
        }

        public long CombineAndStore(SetOperation operation, string destination, string another,
            CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetCombineAndStore(operation, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}",
            _keyName, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{another}", flags);

        public long CombineAndStore(SetOperation operation, string destination, string[] keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(k => $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{k}").ToList();
            redisKeys.AddIfNotContains(_keyName);
            return Wrapper.Database.Value.SetCombineAndStore(operation, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}",
                redisKeys.Select(_ => (RedisKey)_).ToArray(), flags);
        }

        public Task<long> CombineAndStoreAsync(SetOperation operation, string destination, string another, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetCombineAndStoreAsync(operation, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}",
            _keyName, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{another}", flags);

        public async Task<long> CombineAndStoreAsync(SetOperation operation, string destination, string[] keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(k => $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{k}").ToList();
            redisKeys.AddIfNotContains(_keyName);
            return await Wrapper.Database.Value.SetCombineAndStoreAsync(operation, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}",
                redisKeys.Select(_ => (RedisKey)_).ToArray(), flags);
        }

        public async Task<T[]> CombineAsync(SetOperation operation, string another, CommandFlags flags = CommandFlags.None)
            => Wrapper.Unwrap<T>(await Wrapper.Database.Value.SetCombineAsync(operation, _keyName, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{another}", flags));

        public async Task<T[]> CombineAsync(SetOperation operation, string[] keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(k => $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{k}").ToList();
            redisKeys.AddIfNotContains(_keyName);
            return Wrapper.Unwrap<T>(await Wrapper.Database.Value.SetCombineAsync(operation,
                redisKeys.Select(k => (RedisKey)k).ToArray()));
        }

        public bool Contains(T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetContains(_keyName, Wrapper.Wrap(value), flags);

        public Task<bool> ContainsAsync(T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetContainsAsync(_keyName, Wrapper.Wrap(value), flags);

        public long Length(CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetLength(_keyName, flags);

        public Task<long> LengthAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetLengthAsync(_keyName, flags);

        public T[] Members(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SetMembers(_keyName, flags));

        public async Task<T[]> MembersAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.Value.SetMembersAsync(_keyName, flags));

        public bool Move(string destination, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetMove(_keyName, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}", Wrapper.Wrap(value), flags);

        public Task<bool> MoveAsync(string destination, T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetMoveAsync(_keyName, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}", Wrapper.Wrap(value), flags);

        public T Pop(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SetPop(_keyName, flags));

        public async Task<T> PopAsync(CommandFlags flags = CommandFlags.None)
            => Wrapper.Unwrap<T>(await Wrapper.Database.Value.SetPopAsync(_keyName, flags));

        public T RandomMember(CommandFlags flags = CommandFlags.None)
            => Wrapper.Unwrap<T>(Wrapper.Database.Value.SetRandomMember(_keyName, flags));

        public async Task<T> RandomMemberAsync(CommandFlags flags = CommandFlags.None)
            => Wrapper.Unwrap<T>(await Wrapper.Database.Value.SetRandomMemberAsync(_keyName, flags));

        public T[] RandomMembers(long count, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SetRandomMembers(_keyName, count, flags));

        public async Task<T[]> RandomMembersAsync(long count, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.Value.SetRandomMembersAsync(_keyName, count, flags));

        public bool Remove(T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetRemove(_keyName, Wrapper.Wrap(value), flags);

        public long Remove(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetRemove(_keyName, Wrapper.Wrap(values), flags);

        public Task<bool> RemoveAsync(T value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetRemoveAsync(_keyName, Wrapper.Wrap(value), flags);

        public Task<long> RemoveAsync(T[] values, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SetRemoveAsync(_keyName, Wrapper.Wrap(values), flags);

        public T[] Scan(T pattern, int pageSize, CommandFlags flags) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SetScan(_keyName, Wrapper.Wrap(pattern), pageSize, flags).ToArray());

        public T[] Scan(T pattern = default(T), int pageSize = 10, long cursor = 0, int pageOffset = 0,
            CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SetScan(_keyName, Wrapper.Wrap(pattern), pageSize, cursor, pageOffset, flags).ToArray());

        public T[] Sort(long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric,
            T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.Value.Sort(_keyName, skip, take, order, sortType, Wrapper.Wrap(by), get?.Select(_ => Wrapper.Wrap(_)).ToArray(), flags));

        public long SortAndStore(string destination, long skip = 0, long take = -1, Order order = Order.Ascending,
            SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortAndStore(_keyName, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}", skip, take, order, sortType, Wrapper.Wrap(by),
            get?.Select(_ => Wrapper.Wrap(_)).ToArray(), flags);

        public Task<long> SortAndStoreAsync(string destination, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortAndStoreAsync(_keyName, $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{destination}", skip, take, order, sortType, Wrapper.Wrap(by),
            get?.Select(_ => Wrapper.Wrap(_)).ToArray(), flags);

        public async Task<T[]> SortAsync(long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.Value.SortAsync(_keyName, skip, take, order, sortType, Wrapper.Wrap(by), get?.Select(_ => Wrapper.Wrap(_)).ToArray(), flags));
    }
}
