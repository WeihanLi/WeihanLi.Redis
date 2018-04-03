using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class RankClient<T> : BaseRedisClient, IRankClient<T>
    {
        private readonly string _realKey;

        public RankClient(string rankName) : base(LogHelper.GetLogHelper<RankClient<T>>(), new RedisWrapper(RedisConstants.RankPrefix))
        {
            _realKey = Wrapper.GetRealKey(rankName);
        }

        public bool Add(T member, double score, When when = When.Always) => Wrapper.Database.SortedSetAdd(_realKey, Wrapper.Wrap(member), score, when);

        public Task<bool> AddAsync(T member, double score, When when = When.Always) => Wrapper.Database.SortedSetAddAsync(_realKey, Wrapper.Wrap(member), score, when);

        public double Decrement(T member, double value) => Wrapper.Database.SortedSetDecrement(_realKey, Wrapper.Wrap(member), value);

        public Task<double> DecrementAsync(T member, double value) => Wrapper.Database.SortedSetDecrementAsync(_realKey, Wrapper.Wrap(member), value);

        public double Increment(T member, double value) => Wrapper.Database.SortedSetIncrement(_realKey, Wrapper.Wrap(member), value);

        public Task<double> IncrementAsync(T member, double value) => Wrapper.Database.SortedSetIncrementAsync(_realKey, Wrapper.Wrap(member), value);

        public long Length(double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None) => Wrapper.Database.SortedSetLength(_realKey, min, max, exclude);

        public Task<long> LengthAsync(double min = double.NegativeInfinity, double max = double.PositiveInfinity,
            Exclude exclude = Exclude.None) => Wrapper.Database.SortedSetLengthAsync(_realKey, min, max, exclude);

        public T[] RangeByScore(double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) => Wrapper.Unwrap<T>(Wrapper.Database.SortedSetRangeByScore(_realKey, start, stop, exclude, order, skip, take));

        public Task<T[]> RangeByScoreAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) => Wrapper.UnwrapAsync<T>(() => Wrapper.Database.SortedSetRangeByScoreAsync(_realKey, start, stop, exclude, order, skip, take));

        public bool Remove(T member) => Wrapper.Database.SortedSetRemove(_realKey, Wrapper.Wrap(member));

        public Task<bool> RemoveAsync(T member) => Wrapper.Database.SortedSetRemoveAsync(_realKey, Wrapper.Wrap(member));

        public long Remove(T[] members) => Wrapper.Database.SortedSetRemove(_realKey, Wrapper.Wrap(members));

        public Task<long> RemoveAsync(T[] members) => Wrapper.Database.SortedSetRemoveAsync(_realKey, Wrapper.Wrap(members));

        public long RemoveRangeByScore(double start, double stop, Exclude exclude = Exclude.None) => Wrapper.Database.SortedSetRemoveRangeByScore(_realKey, start, stop, exclude);

        public Task<long> RemoveRangeByScoreAsync(double start, double stop, Exclude exclude = Exclude.None) => Wrapper.Database.SortedSetRemoveRangeByScoreAsync(_realKey, start, stop, exclude);

        public long RemoveRangeByRank(long start, long stop) => Wrapper.Database.SortedSetRemoveRangeByRank(_realKey, start, stop);

        public Task<long> RemoveRangeByRankAsync(long start, long stop) => Wrapper.Database.SortedSetRemoveRangeByRankAsync(_realKey, start, stop);

        public double? Score(T member) => Wrapper.Database.SortedSetScore(_realKey, Wrapper.Wrap(member));

        public Task<double?> ScoreAsync(T member) => Wrapper.Database.SortedSetScoreAsync(_realKey, Wrapper.Wrap(member));
    }
}
