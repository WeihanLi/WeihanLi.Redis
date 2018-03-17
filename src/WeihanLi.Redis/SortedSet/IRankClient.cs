using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface IRankClient<T> : IRedisClient
    {
        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key. If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>True if the value was added, False if it already existed (the score is still updated)</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        bool Add(T member, double score, When when = When.Always);

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key. If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>True if the value was added, False if it already existed (the score is still updated)</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        Task<bool> AddAsync(T member, double score, When when = When.Always);

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement. If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <returns>the new score of member</returns>
        /// <remarks>http://redis.io/commands/zincrby</remarks>
        double Decrement(T member, double value);

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement. If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <returns>the new score of member</returns>
        /// <remarks>http://redis.io/commands/zincrby</remarks>
        Task<double> DecrementAsync(T member, double value);

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <returns>the new score of member</returns>
        /// <remarks>http://redis.io/commands/zincrby</remarks>
        double Increment(T member, double value);

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <returns>the new score of member</returns>
        /// <remarks>http://redis.io/commands/zincrby</remarks>
        Task<double> IncrementAsync(T member, double value);

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
        /// </summary>
        /// <returns>the cardinality (number of elements) of the sorted set, or 0 if key does not exist.</returns>
        /// <remarks>http://redis.io/commands/zcard</remarks>
        long Length(double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None);

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
        /// </summary>
        /// <returns>the cardinality (number of elements) of the sorted set, or 0 if key does not exist.</returns>
        /// <remarks>http://redis.io/commands/zcard</remarks>
        Task<long> LengthAsync(double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values. Similar to other range methods the values are inclusive.
        /// </summary>
        /// <returns>list of elements in the specified score range</returns>
        /// <remarks>http://redis.io/commands/zrangebyscore</remarks>
        /// <remarks>http://redis.io/commands/zrevrangebyscore</remarks>
        T[] RangeByScore(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values. Similar to other range methods the values are inclusive.
        /// </summary>
        /// <returns>list of elements in the specified score range</returns>
        /// <remarks>http://redis.io/commands/zrangebyscore</remarks>
        /// <remarks>http://redis.io/commands/zrevrangebyscore</remarks>
        Task<T[]> RangeByScoreAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

        /// <summary>
        /// Removes the specified member from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <returns>True if the member existed in the sorted set and was removed; False otherwise.</returns>
        /// <remarks>http://redis.io/commands/zrem</remarks>
        bool Remove(T member);

        /// <summary>
        /// Removes the specified member from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <returns>True if the member existed in the sorted set and was removed; False otherwise.</returns>
        /// <remarks>http://redis.io/commands/zrem</remarks>
        Task<bool> RemoveAsync(T member);

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <returns>The number of members removed from the sorted set, not including non existing members.</returns>
        /// <remarks>http://redis.io/commands/zrem</remarks>
        long Remove(T[] members);

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <returns>The number of members removed from the sorted set, not including non existing members.</returns>
        /// <remarks>http://redis.io/commands/zrem</remarks>
        Task<long> RemoveAsync(T[] members);

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <returns> the number of elements removed.</returns>
        /// <remarks>http://redis.io/commands/zremrangebyscore</remarks>
        long RemoveRangeByScore(double start, double stop, Exclude exclude = Exclude.None);

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <returns> the number of elements removed.</returns>
        /// <remarks>http://redis.io/commands/zremrangebyscore</remarks>
        Task<long> RemoveRangeByScoreAsync(double start, double stop, Exclude exclude = Exclude.None);

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <returns> the number of elements removed.</returns>
        /// <remarks>http://redis.io/commands/zremrangebyrank</remarks>
        long RemoveRangeByRank(long start, long stop);

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <returns> the number of elements removed.</returns>
        /// <remarks>http://redis.io/commands/zremrangebyrank</remarks>
        Task<long> RemoveRangeByRankAsync(long start, long stop);

        /// <summary>
        /// Returns the score of member in the sorted set at key; If member does not exist in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <returns>the score of member</returns>
        /// <remarks>http://redis.io/commands/zscore</remarks>
        double? Score(T member);

        /// <summary>
        /// Returns the score of member in the sorted set at key; If member does not exist in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <returns>the score of member</returns>
        /// <remarks>http://redis.io/commands/zscore</remarks>
        Task<double?> ScoreAsync(T member);
    }
}
