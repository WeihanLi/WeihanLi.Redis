using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class SortedSetClient<T> : BaseRedisClient, ISortedSetClient<T>
    {
        private readonly string _realKey;

        public SortedSetClient(string keyName) : base(LogHelper.GetLogHelper<SortedSetClient<T>>(), new RedisWrapper("SortedSet"))
        {
            _realKey = $"{Wrapper.KeyPrefix}/{keyName}";
        }

        public bool Add(T member, double score, CommandFlags flags) => Wrapper.Database.SortedSetAdd(_realKey, Wrapper.Wrap(member), score, flags);

        public bool Add(T member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetAdd(_realKey, Wrapper.Wrap(member), score, when, flags);

        public long Add(SortedSetEntry[] values, CommandFlags flags) => Wrapper.Database.SortedSetAdd(_realKey, values, flags);

        public long Add(SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetAdd(_realKey, values, when, flags);

        public Task<bool> AddAsync(T member, double score, CommandFlags flags) => Wrapper.Database.SortedSetAddAsync(_realKey, Wrapper.Wrap(member), score, flags);

        public Task<bool> AddAsync(T member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetAddAsync(_realKey, Wrapper.Wrap(member), score, when, flags);

        public Task<long> AddAsync(SortedSetEntry[] values, CommandFlags flags) => Wrapper.Database.SortedSetAddAsync(_realKey, values, flags);

        public Task<long> AddAsync(SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetAddAsync(_realKey, values, when, flags);

        public long CombineAndStore(SetOperation operation, string destination, string another,
            Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetCombineAndStore(operation, Wrapper.GetRealKey(destination), _realKey,
            Wrapper.GetRealKey(another), aggregate, flags);

        public long CombineAndStore(SetOperation operation, string destination, string[] keys, double[] weights = null,
            Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(_ => Wrapper.GetRealKey(_)).ToList();
            redisKeys.AddIfNotContains(_realKey);
            return Wrapper.Database.SortedSetCombineAndStore(operation, Wrapper.GetRealKey(destination), redisKeys.Select(_ => (RedisKey)_).ToArray(), weights, aggregate, flags);
        }

        public Task<long> CombineAndStoreAsync(SetOperation operation, RedisKey destination, string another, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetCombineAndStoreAsync(operation, Wrapper.GetRealKey(destination), _realKey,
            Wrapper.GetRealKey(another), aggregate, flags);

        public async Task<long> CombineAndStoreAsync(SetOperation operation, string destination, string[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(_ => Wrapper.GetRealKey(_)).ToList();
            redisKeys.AddIfNotContains(_realKey);
            return await Wrapper.Database.SortedSetCombineAndStoreAsync(operation, Wrapper.GetRealKey(destination), redisKeys.Select(_ => (RedisKey)_).ToArray(), weights, aggregate, flags);
        }

        public double Decrement(T member, double value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetDecrement(_realKey, Wrapper.Wrap(member), value, flags);

        public Task<double> DecrementAsync(T member, double value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetDecrementAsync(_realKey, Wrapper.Wrap(member), value, flags);

        public double Increment(T member, double value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetIncrement(_realKey, Wrapper.Wrap(member), value, flags);

        public Task<double> IncrementAsync(T member, double value, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetIncrementAsync(_realKey, Wrapper.Wrap(member), value, flags);

        public long Length(double min = double.NegativeInfinity, double max = double.PositiveInfinity,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetLength(_realKey, min, max, exclude, flags);

        public Task<long> LengthAsync(double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetLengthAsync(_realKey, min, max, exclude, flags);

        public long LengthByValue(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetLengthByValue(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude,
            flags);

        public Task<long> LengthByValueAsync(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetLengthByValueAsync(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude,
            flags);

        public T[] RangeByRank(long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.SortedSetRangeByRank(_realKey, start, stop, order, flags));

        public async Task<T[]> RangeByRankAsync(long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.SortedSetRangeByRankAsync(_realKey, start, stop, order, flags));

        public SortedSetEntry[] RangeByRankWithScores(long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRangeByRankWithScores(_realKey, start, stop, order, flags);

        public Task<SortedSetEntry[]> RangeByRankWithScoresAsync(long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRangeByRankWithScoresAsync(_realKey, start, stop, order, flags);

        public T[] RangeByScore(double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1,
            CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(
            Wrapper.Database.SortedSetRangeByScore(_realKey, start, stop, exclude, order, skip, take, flags));

        public async Task<T[]> RangeByScoreAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(
            await Wrapper.Database.SortedSetRangeByScoreAsync(_realKey, start, stop, exclude, order, skip, take, flags));

        public SortedSetEntry[] RangeByScoreWithScores(double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0,
            long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRangeByScoreWithScores(_realKey, start, stop, exclude, order, skip, take,
            flags);

        public Task<SortedSetEntry[]> RangeByScoreWithScoresAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRangeByScoreWithScoresAsync(_realKey, start, stop, exclude, order, skip, take,
            flags);

        public T[] RangeByValue(T min = default(T), T max = default(T), Exclude exclude = Exclude.None, long skip = 0,
            long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(Wrapper.Database.SortedSetRangeByValue(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude, skip, take, flags));

        public async Task<T[]> RangeByValueAsync(T min = default(T), T max = default(T), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<T>(await Wrapper.Database.SortedSetRangeByValueAsync(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude, skip, take, flags));

        public long? Rank(T member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRank(_realKey, Wrapper.Wrap(member), order, flags);

        public Task<long?> RankAsync(T member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRankAsync(_realKey, Wrapper.Wrap(member), order, flags);

        public bool Remove(T member, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemove(_realKey, Wrapper.Wrap(member), flags);

        public long Remove(T[] members, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemove(_realKey, Wrapper.Wrap(members), flags);

        public Task<bool> RemoveAsync(T member, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemoveAsync(_realKey, Wrapper.Wrap(member), flags);

        public Task<long> RemoveAsync(T[] members, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemoveAsync(_realKey, Wrapper.Wrap(members), flags);

        public long RemoveRangeByRank(long start, long stop, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemoveRangeByRank(_realKey, start, stop, flags);

        public Task<long> RemoveRangeByRankAsync(long start, long stop, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemoveRangeByRankAsync(_realKey, start, stop, flags);

        public long RemoveRangeByScore(double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemoveRangeByScore(_realKey, start, stop, exclude, flags);

        public Task<long> RemoveRangeByScoreAsync(double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemoveRangeByScoreAsync(_realKey, start, stop, exclude, flags);

        public long RemoveRangeByValue(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemoveRangeByValue(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude, flags);

        public Task<long> RemoveRangeByValueAsync(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetRemoveRangeByValueAsync(_realKey, Wrapper.Wrap(min), Wrapper.Wrap(max), exclude, flags);

        public IEnumerable<SortedSetEntry> Scan(T pattern, int pageSize, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetScan(_realKey, Wrapper.Wrap(pattern), pageSize, flags);

        public IEnumerable<SortedSetEntry> Scan(T pattern = default(T), int pageSize = 10, long cursor = 0, int pageOffset = 0,
            CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetScan(_realKey, Wrapper.Wrap(pattern), pageSize, cursor, pageOffset, flags);

        public double? Score(T member, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetScore(_realKey, Wrapper.Wrap(member), flags);

        public Task<double?> ScoreAsync(T member, CommandFlags flags = CommandFlags.None) => Wrapper.Database.SortedSetScoreAsync(_realKey, Wrapper.Wrap(member), flags);
    }
}
