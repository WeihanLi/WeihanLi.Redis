using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface ISortedSetClient<T> : IRedisClient
    {
        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key. If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>True if the value was added, False if it already existed (the score is still updated)</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        bool Add(T member, double score, CommandFlags flags);

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key. If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>True if the value was added, False if it already existed (the score is still updated)</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        bool Add(T member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key. If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>The number of elements added to the sorted sets, not including elements already existing for which the score was updated.</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        long Add(SortedSetEntry[] values, CommandFlags flags);

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key. If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>The number of elements added to the sorted sets, not including elements already existing for which the score was updated.</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        long Add(SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Computes a set operation over two sorted sets, and stores the result in destination, optionally performing
        /// a specific aggregation (defaults to sum)
        /// </summary>
        /// <remarks>http://redis.io/commands/zunionstore</remarks>
        /// <remarks>http://redis.io/commands/zinterstore</remarks>
        /// <returns>the number of elements in the resulting sorted set at destination</returns>
        long CombineAndStore(SetOperation operation, string destination, string another, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Computes a set operation over multiple sorted sets (optionally using per-set weights), and stores the result in destination, optionally performing
        /// a specific aggregation (defaults to sum)
        /// </summary>
        /// <remarks>http://redis.io/commands/zunionstore</remarks>
        /// <remarks>http://redis.io/commands/zinterstore</remarks>
        /// <returns>the number of elements in the resulting sorted set at destination</returns>
        long CombineAndStore(SetOperation operation, string destination, string[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement. If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <returns>the new score of member</returns>
        /// <remarks>http://redis.io/commands/zincrby</remarks>
        double Decrement(T member, double value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <returns>the new score of member</returns>
        /// <remarks>http://redis.io/commands/zincrby</remarks>
        double Increment(T member, double value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
        /// </summary>
        /// <returns>the cardinality (number of elements) of the sorted set, or 0 if key does not exist.</returns>
        /// <remarks>http://redis.io/commands/zcard</remarks>
        long Length(double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns the number of elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <returns>the number of elements in the specified score range.</returns>
        /// <remarks>When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max.</remarks>
        long LengthByValue(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on. They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <returns>list of elements in the specified range</returns>
        /// <remarks>http://redis.io/commands/zrange</remarks>
        /// <remarks>http://redis.io/commands/zrevrange</remarks>
        T[] RangeByRank(long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on. They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <returns>list of elements in the specified range</returns>
        /// <remarks>http://redis.io/commands/zrange</remarks>
        /// <remarks>http://redis.io/commands/zrevrange</remarks>
        SortedSetEntry[] RangeByRankWithScores(long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values. Similar to other range methods the values are inclusive.
        /// </summary>
        /// <returns>list of elements in the specified score range</returns>
        /// <remarks>http://redis.io/commands/zrangebyscore</remarks>
        /// <remarks>http://redis.io/commands/zrevrangebyscore</remarks>
        T[] RangeByScore(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values. Similar to other range methods the values are inclusive.
        /// </summary>
        /// <returns>list of elements in the specified score range</returns>
        /// <remarks>http://redis.io/commands/zrangebyscore</remarks>
        /// <remarks>http://redis.io/commands/zrevrangebyscore</remarks>
        SortedSetEntry[] RangeByScoreWithScores(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <remarks>http://redis.io/commands/zrangebylex</remarks>
        /// <returns>list of elements in the specified score range.</returns>
        T[] RangeByValue(T min = default(T), T max = default(T), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, by default with the scores ordered from low to high. The rank (or index) is 0-based, which means that the member with the lowest score has rank 0.
        /// </summary>
        /// <returns>If member exists in the sorted set, the rank of member; If member does not exist in the sorted set or key does not exist, null</returns>
        /// <remarks>http://redis.io/commands/zrank</remarks>
        /// <remarks>http://redis.io/commands/zrevrank</remarks>
        long? Rank(T member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes the specified member from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <returns>True if the member existed in the sorted set and was removed; False otherwise.</returns>
        /// <remarks>http://redis.io/commands/zrem</remarks>
        bool Remove(T member, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <returns>The number of members removed from the sorted set, not including non existing members.</returns>
        /// <remarks>http://redis.io/commands/zrem</remarks>
        long Remove(T[] members, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <returns> the number of elements removed.</returns>
        /// <remarks>http://redis.io/commands/zremrangebyrank</remarks>
        long RemoveRangeByRank(long start, long stop, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <returns> the number of elements removed.</returns>
        /// <remarks>http://redis.io/commands/zremrangebyscore</remarks>
        long RemoveRangeByScore(double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command removes all elements in the sorted set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <remarks>http://redis.io/commands/zremrangebylex</remarks>
        /// <returns>the number of elements removed.</returns>
        long RemoveRangeByValue(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// The ZSCAN command is used to incrementally iterate over a sorted set
        /// </summary>
        /// <returns>yields all elements of the sorted set.</returns>
        /// <remarks>http://redis.io/commands/zscan</remarks>
        IEnumerable<SortedSetEntry> Scan(T pattern, int pageSize, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// The ZSCAN command is used to incrementally iterate over a sorted set; note: to resume an iteration via <i>cursor</i>, cast the original enumerable or enumerator to <i>IScanningCursor</i>.
        /// </summary>
        /// <returns>yields all elements of the sorted set.</returns>
        /// <remarks>http://redis.io/commands/zscan</remarks>
        IEnumerable<SortedSetEntry> Scan(T pattern = default(T), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the score of member in the sorted set at key; If member does not exist in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <returns>the score of member</returns>
        /// <remarks>http://redis.io/commands/zscore</remarks>
        double? Score(T member, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key. If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>True if the value was added, False if it already existed (the score is still updated)</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        Task<bool> AddAsync(T member, double score, CommandFlags flags);

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key. If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>True if the value was added, False if it already existed (the score is still updated)</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        Task<bool> AddAsync(T member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key. If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>The number of elements added to the sorted sets, not including elements already existing for which the score was updated.</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        Task<long> AddAsync(SortedSetEntry[] values, CommandFlags flags);

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key. If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <returns>The number of elements added to the sorted sets, not including elements already existing for which the score was updated.</returns>
        /// <remarks>http://redis.io/commands/zadd</remarks>
        Task<long> AddAsync(SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Computes a set operation over two sorted sets, and stores the result in destination, optionally performing
        /// a specific aggregation (defaults to sum)
        /// </summary>
        /// <remarks>http://redis.io/commands/zunionstore</remarks>
        /// <remarks>http://redis.io/commands/zinterstore</remarks>
        /// <returns>the number of elements in the resulting sorted set at destination</returns>
        Task<long> CombineAndStoreAsync(SetOperation operation, RedisKey destination, string another, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Computes a set operation over multiple sorted sets (optionally using per-set weights), and stores the result in destination, optionally performing
        /// a specific aggregation (defaults to sum)
        /// </summary>
        /// <remarks>http://redis.io/commands/zunionstore</remarks>
        /// <remarks>http://redis.io/commands/zinterstore</remarks>
        /// <returns>the number of elements in the resulting sorted set at destination</returns>
        Task<long> CombineAndStoreAsync(SetOperation operation, string destination, string[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement. If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <returns>the new score of member</returns>
        /// <remarks>http://redis.io/commands/zincrby</remarks>
        Task<double> DecrementAsync(T member, double value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <returns>the new score of member</returns>
        /// <remarks>http://redis.io/commands/zincrby</remarks>
        Task<double> IncrementAsync(T member, double value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
        /// </summary>
        /// <returns>the cardinality (number of elements) of the sorted set, or 0 if key does not exist.</returns>
        /// <remarks>http://redis.io/commands/zcard</remarks>
        Task<long> LengthAsync(double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns the number of elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <returns>the number of elements in the specified score range.</returns>
        /// <remarks>When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max.</remarks>
        Task<long> LengthByValueAsync(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on. They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <returns>list of elements in the specified range</returns>
        /// <remarks>http://redis.io/commands/zrange</remarks>
        /// <remarks>http://redis.io/commands/zrevrange</remarks>
        Task<T[]> RangeByRankAsync(long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on. They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <returns>list of elements in the specified range</returns>
        /// <remarks>http://redis.io/commands/zrange</remarks>
        /// <remarks>http://redis.io/commands/zrevrange</remarks>
        Task<SortedSetEntry[]> RangeByRankWithScoresAsync(long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values. Similar to other range methods the values are inclusive.
        /// </summary>
        /// <returns>list of elements in the specified score range</returns>
        /// <remarks>http://redis.io/commands/zrangebyscore</remarks>
        /// <remarks>http://redis.io/commands/zrevrangebyscore</remarks>
        Task<T[]> RangeByScoreAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values. Similar to other range methods the values are inclusive.
        /// </summary>
        /// <returns>list of elements in the specified score range</returns>
        /// <remarks>http://redis.io/commands/zrangebyscore</remarks>
        /// <remarks>http://redis.io/commands/zrevrangebyscore</remarks>
        Task<SortedSetEntry[]> RangeByScoreWithScoresAsync(double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command returns all the elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <remarks>http://redis.io/commands/zrangebylex</remarks>
        /// <returns>list of elements in the specified score range.</returns>
        Task<T[]> RangeByValueAsync(T min = default(T), T max = default(T), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, by default with the scores ordered from low to high. The rank (or index) is 0-based, which means that the member with the lowest score has rank 0.
        /// </summary>
        /// <returns>If member exists in the sorted set, the rank of member; If member does not exist in the sorted set or key does not exist, null</returns>
        /// <remarks>http://redis.io/commands/zrank</remarks>
        /// <remarks>http://redis.io/commands/zrevrank</remarks>
        Task<long?> RankAsync(T member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes the specified member from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <returns>True if the member existed in the sorted set and was removed; False otherwise.</returns>
        /// <remarks>http://redis.io/commands/zrem</remarks>
        Task<bool> RemoveAsync(T member, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <returns>The number of members removed from the sorted set, not including non existing members.</returns>
        /// <remarks>http://redis.io/commands/zrem</remarks>
        Task<long> RemoveAsync(T[] members, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <returns> the number of elements removed.</returns>
        /// <remarks>http://redis.io/commands/zremrangebyrank</remarks>
        Task<long> RemoveRangeByRankAsync(long start, long stop, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <returns> the number of elements removed.</returns>
        /// <remarks>http://redis.io/commands/zremrangebyscore</remarks>
        Task<long> RemoveRangeByScoreAsync(double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command removes all elements in the sorted set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <remarks>http://redis.io/commands/zremrangebylex</remarks>
        /// <returns>the number of elements removed.</returns>
        Task<long> RemoveRangeByValueAsync(T min, T max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the score of member in the sorted set at key; If member does not exist in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <returns>the score of member</returns>
        /// <remarks>http://redis.io/commands/zscore</remarks>
        Task<double?> ScoreAsync(T member, CommandFlags flags = CommandFlags.None);
    }
}
