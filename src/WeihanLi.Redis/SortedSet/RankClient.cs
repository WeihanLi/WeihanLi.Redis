using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class RankClient<T> : BaseRedisClient, IRankClient<T>
    {
        private readonly string _realKey;

        public RankClient(string rankName, ILogger<RankClient<T>> logger) : base(logger, new RedisWrapper(RedisConstants.RankPrefix))
        {
            _realKey = Wrapper.GetRealKey(rankName);
        }

        public bool Add(T member, double score, When when = When.Always) => Wrapper.Database.Value.SortedSetAdd(_realKey, Wrapper.Wrap(member), score, when);

        public Task<bool> AddAsync(T member, double score, When when = When.Always) => Wrapper.Database.Value.SortedSetAddAsync(_realKey, Wrapper.Wrap(member), score, when);

        public long Add(IDictionary<T, double> values, When when = When.Always)
            => Wrapper.Database.Value.SortedSetAdd(_realKey, values.Select(_ => new SortedSetEntry(Wrapper.Wrap(_.Key), _.Value)).ToArray(), when);

        public Task<long> AddAsync(IDictionary<T, double> values, When when = When.Always)
            => Wrapper.Database.Value.SortedSetAddAsync(_realKey, values.Select(_ => new SortedSetEntry(Wrapper.Wrap(_.Key), _.Value)).ToArray(), when);

        public double Decrement(T member, double value) => Wrapper.Database.Value.SortedSetDecrement(_realKey, Wrapper.Wrap(member), value);

        public Task<double> DecrementAsync(T member, double value) => Wrapper.Database.Value.SortedSetDecrementAsync(_realKey, Wrapper.Wrap(member), value);

        public double Increment(T member, double value) => Wrapper.Database.Value.SortedSetIncrement(_realKey, Wrapper.Wrap(member), value);

        public Task<double> IncrementAsync(T member, double value) => Wrapper.Database.Value.SortedSetIncrementAsync(_realKey, Wrapper.Wrap(member), value);

        public long Length(double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None) => Wrapper.Database.Value.SortedSetLength(_realKey, min, max, exclude);

        public Task<long> LengthAsync(double min = double.NegativeInfinity, double max = double.PositiveInfinity,
            Exclude exclude = Exclude.None) => Wrapper.Database.Value.SortedSetLengthAsync(_realKey, min, max, exclude);

        public T[] RangeByScore(double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) => Wrapper.Unwrap<T>(Wrapper.Database.Value.SortedSetRangeByScore(_realKey, start, stop, exclude, order, skip, take));

        public Task<T[]> RangeByScoreAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.Value.SortedSetRangeByScoreAsync(_realKey, start, stop, exclude, order, skip, take));

        public bool Remove(T member) => Wrapper.Database.Value.SortedSetRemove(_realKey, Wrapper.Wrap(member));

        public Task<bool> RemoveAsync(T member) => Wrapper.Database.Value.SortedSetRemoveAsync(_realKey, Wrapper.Wrap(member));

        public long Remove(T[] members) => Wrapper.Database.Value.SortedSetRemove(_realKey, Wrapper.Wrap(members));

        public Task<long> RemoveAsync(T[] members) => Wrapper.Database.Value.SortedSetRemoveAsync(_realKey, Wrapper.Wrap(members));

        public long RemoveRangeByScore(double start, double stop, Exclude exclude = Exclude.None) => Wrapper.Database.Value.SortedSetRemoveRangeByScore(_realKey, start, stop, exclude);

        public Task<long> RemoveRangeByScoreAsync(double start, double stop, Exclude exclude = Exclude.None) => Wrapper.Database.Value.SortedSetRemoveRangeByScoreAsync(_realKey, start, stop, exclude);

        public long RemoveRangeByRank(long start, long stop) => Wrapper.Database.Value.SortedSetRemoveRangeByRank(_realKey, start, stop);

        public Task<long> RemoveRangeByRankAsync(long start, long stop) => Wrapper.Database.Value.SortedSetRemoveRangeByRankAsync(_realKey, start, stop);

        public double? Score(T member) => Wrapper.Database.Value.SortedSetScore(_realKey, Wrapper.Wrap(member));

        public Task<double?> ScoreAsync(T member) => Wrapper.Database.Value.SortedSetScoreAsync(_realKey, Wrapper.Wrap(member));
    }
}
