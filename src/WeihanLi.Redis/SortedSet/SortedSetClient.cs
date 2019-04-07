using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Extensions;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class SortedSetClient<T> : BaseRedisClient, ISortedSetClient<T>
    {
        private readonly string _realKey;

        public SortedSetClient(string keyName, ILogger<SortedSetClient<T>> logger) : base(logger, new RedisWrapper(RedisConstants.SortedSetPrefix))
        {
            _realKey = $"{Wrapper.KeyPrefix}{RedisManager.RedisConfiguration.KeySeparator}{keyName}";
        }

        public bool Add(T member, double score, CommandFlags flags) => Wrapper.Database.Value.SortedSetAdd(_realKey, Wrapper.Wrap(member), score, flags);

        public bool Add(T member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetAdd(_realKey, Wrapper.Wrap(member), score, when, flags);

        public long Add(SortedSetEntry[] values, CommandFlags flags) => Wrapper.Database.Value.SortedSetAdd(_realKey, values, flags);

        public long Add(SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetAdd(_realKey, values, when, flags);

        public Task<bool> AddAsync(T member, double score, CommandFlags flags) => Wrapper.Database.Value.SortedSetAddAsync(_realKey, Wrapper.Wrap(member), score, flags);

        public Task<bool> AddAsync(T member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetAddAsync(_realKey, Wrapper.Wrap(member), score, when, flags);

        public Task<long> AddAsync(SortedSetEntry[] values, CommandFlags flags) => Wrapper.Database.Value.SortedSetAddAsync(_realKey, values, flags);

        public Task<long> AddAsync(SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetAddAsync(_realKey, values, when, flags);

        public long CombineAndStore(SetOperation operation, string destination, string another,
            Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetCombineAndStore(operation, Wrapper.GetRealKey(destination), _realKey,
            Wrapper.GetRealKey(another), aggregate, flags);

        public long CombineAndStore(SetOperation operation, string destination, string[] keys, double[] weights = null,
            Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(_ => Wrapper.GetRealKey(_)).ToList();
            redisKeys.AddIfNotContains(_realKey);
            return Wrapper.Database.Value.SortedSetCombineAndStore(operation, Wrapper.GetRealKey(destination), redisKeys.Select(_ => (RedisKey)_).ToArray(), weights, aggregate, flags);
        }

        public Task<long> CombineAndStoreAsync(SetOperation operation, RedisKey destination, string another, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetCombineAndStoreAsync(operation, Wrapper.GetRealKey(destination), _realKey,
            Wrapper.GetRealKey(another), aggregate, flags);

        public async Task<long> CombineAndStoreAsync(SetOperation operation, string destination, string[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(_ => Wrapper.GetRealKey(_)).ToList();
            redisKeys.AddIfNotContains(_realKey);
            return await Wrapper.Database.Value.SortedSetCombineAndStoreAsync(operation, Wrapper.GetRealKey(destination), redisKeys.Select(_ => (RedisKey)_).ToArray(), weights, aggregate, flags);
        }

        public double Decrement(T member, double value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetDecrement(_realKey, Wrapper.Wrap(member), value, flags);

        public Task<double> DecrementAsync(T member, double value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetDecrementAsync(_realKey, Wrapper.Wrap(member), value, flags);

        public double Increment(T member, double value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetIncrement(_realKey, Wrapper.Wrap(member), value, flags);

        public Task<double> IncrementAsync(T member, double value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetIncrementAsync(_realKey, Wrapper.Wrap(member), value, flags);

        public long Length(double min = double.NegativeInfinity, double max = double.PositiveInfinity,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetLength(_realKey, min, max, exclude, flags);

        public Task<long> LengthAsync(double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetLengthAsync(_realKey, min, max, exclude, flags);

        public long LengthByValue(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetLengthByValue(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude,
            flags);

        public Task<long> LengthByValueAsync(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetLengthByValueAsync(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude,
            flags);

        public T[] RangeByRank(long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SortedSetRangeByRank(_realKey, start, stop, order, flags));

        public async Task<T[]> RangeByRankAsync(long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.Value.SortedSetRangeByRankAsync(_realKey, start, stop, order, flags));

        public SortedSetEntry[] RangeByRankWithScores(long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRangeByRankWithScores(_realKey, start, stop, order, flags);

        public Task<SortedSetEntry[]> RangeByRankWithScoresAsync(long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRangeByRankWithScoresAsync(_realKey, start, stop, order, flags);

        public T[] RangeByScore(double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1,
            CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(
            Wrapper.Database.Value.SortedSetRangeByScore(_realKey, start, stop, exclude, order, skip, take, flags));

        public async Task<T[]> RangeByScoreAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(
            await Wrapper.Database.Value.SortedSetRangeByScoreAsync(_realKey, start, stop, exclude, order, skip, take, flags));

        public SortedSetEntry[] RangeByScoreWithScores(double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0,
            long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRangeByScoreWithScores(_realKey, start, stop, exclude, order, skip, take,
            flags);

        public Task<SortedSetEntry[]> RangeByScoreWithScoresAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRangeByScoreWithScoresAsync(_realKey, start, stop, exclude, order, skip, take,
            flags);

        public T[] RangeByValue(T min = default(T), T max = default(T), Exclude exclude = Exclude.None, long skip = 0,
            long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SortedSetRangeByValue(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude, skip, take, flags));

        public async Task<T[]> RangeByValueAsync(T min = default(T), T max = default(T), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.Value.SortedSetRangeByValueAsync(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude, skip, take, flags));

        public long? Rank(T member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRank(_realKey, Wrapper.Wrap(member), order, flags);

        public Task<long?> RankAsync(T member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRankAsync(_realKey, Wrapper.Wrap(member), order, flags);

        public bool Remove(T member, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemove(_realKey, Wrapper.Wrap(member), flags);

        public long Remove(T[] members, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemove(_realKey, Wrapper.Wrap(members), flags);

        public Task<bool> RemoveAsync(T member, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemoveAsync(_realKey, Wrapper.Wrap(member), flags);

        public Task<long> RemoveAsync(T[] members, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemoveAsync(_realKey, Wrapper.Wrap(members), flags);

        public long RemoveRangeByRank(long start, long stop, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemoveRangeByRank(_realKey, start, stop, flags);

        public Task<long> RemoveRangeByRankAsync(long start, long stop, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemoveRangeByRankAsync(_realKey, start, stop, flags);

        public long RemoveRangeByScore(double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemoveRangeByScore(_realKey, start, stop, exclude, flags);

        public Task<long> RemoveRangeByScoreAsync(double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemoveRangeByScoreAsync(_realKey, start, stop, exclude, flags);

        public long RemoveRangeByValue(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemoveRangeByValue(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude, flags);

        public Task<long> RemoveRangeByValueAsync(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetRemoveRangeByValueAsync(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude, flags);

        public IEnumerable<SortedSetEntry> Scan(T pattern, int pageSize, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetScan(_realKey, Wrapper.Wrap(pattern), pageSize, flags);

        public IEnumerable<SortedSetEntry> Scan(T pattern = default(T), int pageSize = 10, long cursor = 0, int pageOffset = 0,
            CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetScan(_realKey, Wrapper.Wrap(pattern), pageSize, cursor, pageOffset, flags);

        public double? Score(T member, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetScore(_realKey, Wrapper.Wrap(member), flags);

        public Task<double?> ScoreAsync(T member, CommandFlags flags = CommandFlags.None) => Wrapper.Database.Value.SortedSetScoreAsync(_realKey, Wrapper.Wrap(member), flags);
    }
}
