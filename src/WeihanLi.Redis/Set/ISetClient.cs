using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface ISetClient<T> : IRedisClient
    {
        /// <summary>
        /// Add the specified member to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <returns>True if the specified member was not already present in the set, else False</returns>
        /// <remarks>http://redis.io/commands/sadd</remarks>
        bool Add(T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <returns>the number of elements that were added to the set, not including all the elements already present into the set.</returns>
        /// <remarks>http://redis.io/commands/sadd</remarks>
        long Add(T[] values, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against the given sets.
        /// </summary>
        /// <returns>list with members of the resulting set.</returns>
        /// <remarks>http://redis.io/commands/sunion</remarks>
        /// <remarks>http://redis.io/commands/sinter</remarks>
        /// <remarks>http://redis.io/commands/sdiff</remarks>
        T[] Combine(SetOperation operation, string another, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against the given sets.
        /// </summary>
        /// <returns>list with members of the resulting set.</returns>
        /// <remarks>http://redis.io/commands/sunion</remarks>
        /// <remarks>http://redis.io/commands/sinter</remarks>
        /// <remarks>http://redis.io/commands/sdiff</remarks>
        T[] Combine(SetOperation operation, string[] keys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// This command is equal to SetCombine, but instead of returning the resulting set, it is stored in destination. If destination already exists, it is overwritten.
        /// </summary>
        /// <returns>the number of elements in the resulting set.</returns>
        /// <remarks>http://redis.io/commands/sunionstore</remarks>
        /// <remarks>http://redis.io/commands/sinterstore</remarks>
        /// <remarks>http://redis.io/commands/sdiffstore</remarks>
        long CombineAndStore(SetOperation operation, string destination, string another, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// This command is equal to SetCombine, but instead of returning the resulting set, it is stored in destination. If destination already exists, it is overwritten.
        /// </summary>
        /// <returns>the number of elements in the resulting set.</returns>
        /// <remarks>http://redis.io/commands/sunionstore</remarks>
        /// <remarks>http://redis.io/commands/sinterstore</remarks>
        /// <remarks>http://redis.io/commands/sdiffstore</remarks>
        long CombineAndStore(SetOperation operation, string destination, string[] keys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// </summary>
        /// <returns>1 if the element is a member of the set. 0 if the element is not a member of the set, or if key does not exist.</returns>
        /// <remarks>http://redis.io/commands/sismember</remarks>
        bool Contains(T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <returns>the cardinality (number of elements) of the set, or 0 if key does not exist.</returns>
        /// <remarks>http://redis.io/commands/scard</remarks>
        long Length(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <returns>all elements of the set.</returns>
        /// <remarks>http://redis.io/commands/smembers</remarks>
        T[] Members(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Move member from the set at source to the set at destination. This operation is atomic. In every given moment the element will appear to be a member of source or destination for other clients.
        /// When the specified element already exists in the destination set, it is only removed from the source set.
        /// </summary>
        /// <returns>1 if the element is moved. 0 if the element is not a member of source and no operation was performed.</returns>
        /// <remarks>http://redis.io/commands/smove</remarks>
        bool Move(string destination, T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <returns>the removed element, or nil when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/spop</remarks>
        T Pop(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Return a random element from the set value stored at key.
        /// </summary>
        /// <returns>the randomly selected element, or nil when key does not exist</returns>
        /// <remarks>http://redis.io/commands/srandmember</remarks>
        T RandomMember(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Return an array of count distinct elements if count is positive. If called with a negative count the behavior changes and the command is allowed to return the same element multiple times.
        /// In this case the numer of returned elements is the absolute value of the specified count.
        /// </summary>
        /// <returns>an array of elements, or an empty array when key does not exist</returns>
        /// <remarks>http://redis.io/commands/srandmember</remarks>
        T[] RandomMembers(long count, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Remove the specified member from the set stored at key.  Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <returns>True if the specified member was already present in the set, else False</returns>
        /// <remarks>http://redis.io/commands/srem</remarks>
        bool Remove(T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Remove the specified members from the set stored at key. Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <returns>the number of members that were removed from the set, not including non existing members.</returns>
        /// <remarks>http://redis.io/commands/srem</remarks>
        long Remove(T[] values, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// The SSCAN command is used to incrementally iterate over set
        /// </summary>
        /// <returns>yields all elements of the set.</returns>
        /// <remarks>http://redis.io/commands/sscan</remarks>
        T[] Scan(T pattern, int pageSize, CommandFlags flags);

        /// <summary>
        /// The SSCAN command is used to incrementally iterate over set; note: to resume an iteration via <i>cursor</i>, cast the original enumerable or enumerator to <i>IScanningCursor</i>.
        /// </summary>
        /// <returns>yields all elements of the set.</returns>
        /// <remarks>http://redis.io/commands/sscan</remarks>
        T[] Scan(T pattern = default(T), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by default); By default, the elements themselves are compared, but the values can also be
        /// used to perform external key-lookups using the <c>by</c> parameter. By default, the elements themselves are returned, but external key-lookups (one or many) can
        /// be performed instead by specifying the <c>get</c> parameter (note that <c>#</c> specifies the element itself, when used in <c>get</c>).
        /// Referring to the <a href="http://redis.io/commands/sort">redis SORT documentation </a> for examples is recommended. When used in hashes, <c>by</c> and <c>get</c>
        /// can be used to specify fields using <c>-&gt;</c> notation (again, refer to redis documentation).
        /// </summary>
        /// <remarks>http://redis.io/commands/sort</remarks>
        /// <returns>Returns the sorted elements, or the external values if <c>get</c> is specified</returns>
        T[] Sort(long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by default); By default, the elements themselves are compared, but the values can also be
        /// used to perform external key-lookups using the <c>by</c> parameter. By default, the elements themselves are returned, but external key-lookups (one or many) can
        /// be performed instead by specifying the <c>get</c> parameter (note that <c>#</c> specifies the element itself, when used in <c>get</c>).
        /// Referring to the <a href="http://redis.io/commands/sort">redis SORT documentation </a> for examples is recommended. When used in hashes, <c>by</c> and <c>get</c>
        /// can be used to specify fields using <c>-&gt;</c> notation (again, refer to redis documentation).
        /// </summary>
        /// <remarks>http://redis.io/commands/sort</remarks>
        /// <returns>Returns the number of elements stored in the new list</returns>
        long SortAndStore(string destination, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Add the specified member to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <returns>True if the specified member was not already present in the set, else False</returns>
        /// <remarks>http://redis.io/commands/sadd</remarks>
        Task<bool> AddAsync(T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <returns>the number of elements that were added to the set, not including all the elements already present into the set.</returns>
        /// <remarks>http://redis.io/commands/sadd</remarks>
        Task<long> AddAsync(T[] values, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// This command is equal to SetCombine, but instead of returning the resulting set, it is stored in destination. If destination already exists, it is overwritten.
        /// </summary>
        /// <returns>the number of elements in the resulting set.</returns>
        /// <remarks>http://redis.io/commands/sunionstore</remarks>
        /// <remarks>http://redis.io/commands/sinterstore</remarks>
        /// <remarks>http://redis.io/commands/sdiffstore</remarks>
        Task<long> CombineAndStoreAsync(SetOperation operation, string destination, string another, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// This command is equal to SetCombine, but instead of returning the resulting set, it is stored in destination. If destination already exists, it is overwritten.
        /// </summary>
        /// <returns>the number of elements in the resulting set.</returns>
        /// <remarks>http://redis.io/commands/sunionstore</remarks>
        /// <remarks>http://redis.io/commands/sinterstore</remarks>
        /// <remarks>http://redis.io/commands/sdiffstore</remarks>
        Task<long> CombineAndStoreAsync(SetOperation operation, string destination, string[] keys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against the given sets.
        /// </summary>
        /// <returns>list with members of the resulting set.</returns>
        /// <remarks>http://redis.io/commands/sunion</remarks>
        /// <remarks>http://redis.io/commands/sinter</remarks>
        /// <remarks>http://redis.io/commands/sdiff</remarks>
        Task<T[]> CombineAsync(SetOperation operation, string another, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against the given sets.
        /// </summary>
        /// <returns>list with members of the resulting set.</returns>
        /// <remarks>http://redis.io/commands/sunion</remarks>
        /// <remarks>http://redis.io/commands/sinter</remarks>
        /// <remarks>http://redis.io/commands/sdiff</remarks>
        Task<T[]> CombineAsync(SetOperation operation, string[] keys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// </summary>
        /// <returns>1 if the element is a member of the set. 0 if the element is not a member of the set, or if key does not exist.</returns>
        /// <remarks>http://redis.io/commands/sismember</remarks>
        Task<bool> ContainsAsync(T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <returns>the cardinality (number of elements) of the set, or 0 if key does not exist.</returns>
        /// <remarks>http://redis.io/commands/scard</remarks>
        Task<long> LengthAsync(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <returns>all elements of the set.</returns>
        /// <remarks>http://redis.io/commands/smembers</remarks>
        Task<T[]> MembersAsync(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Move member from the set at source to the set at destination. This operation is atomic. In every given moment the element will appear to be a member of source or destination for other clients.
        /// When the specified element already exists in the destination set, it is only removed from the source set.
        /// </summary>
        /// <returns>1 if the element is moved. 0 if the element is not a member of source and no operation was performed.</returns>
        /// <remarks>http://redis.io/commands/smove</remarks>
        Task<bool> MoveAsync(string destination, T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <returns>the removed element, or nil when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/spop</remarks>
        Task<T> PopAsync(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Return a random element from the set value stored at key.
        /// </summary>
        /// <returns>the randomly selected element, or nil when key does not exist</returns>
        /// <remarks>http://redis.io/commands/srandmember</remarks>
        Task<T> RandomMemberAsync(CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Return an array of count distinct elements if count is positive. If called with a negative count the behavior changes and the command is allowed to return the same element multiple times.
        /// In this case the numer of returned elements is the absolute value of the specified count.
        /// </summary>
        /// <returns>an array of elements, or an empty array when key does not exist</returns>
        /// <remarks>http://redis.io/commands/srandmember</remarks>
        Task<T[]> RandomMembersAsync(long count, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Remove the specified member from the set stored at key.  Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <returns>True if the specified member was already present in the set, else False</returns>
        /// <remarks>http://redis.io/commands/srem</remarks>
        Task<bool> RemoveAsync(T value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Remove the specified members from the set stored at key. Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <returns>the number of members that were removed from the set, not including non existing members.</returns>
        /// <remarks>http://redis.io/commands/srem</remarks>
        Task<long> RemoveAsync(T[] values, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by default); By default, the elements themselves are compared, but the values can also be
        /// used to perform external key-lookups using the <c>by</c> parameter. By default, the elements themselves are returned, but external key-lookups (one or many) can
        /// be performed instead by specifying the <c>get</c> parameter (note that <c>#</c> specifies the element itself, when used in <c>get</c>).
        /// Referring to the <a href="http://redis.io/commands/sort">redis SORT documentation </a> for examples is recommended. When used in hashes, <c>by</c> and <c>get</c>
        /// can be used to specify fields using <c>-&gt;</c> notation (again, refer to redis documentation).
        /// </summary>
        /// <remarks>http://redis.io/commands/sort</remarks>
        /// <returns>Returns the number of elements stored in the new list</returns>
        Task<long> SortAndStoreAsync(string destination, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by default); By default, the elements themselves are compared, but the values can also be
        /// used to perform external key-lookups using the <c>by</c> parameter. By default, the elements themselves are returned, but external key-lookups (one or many) can
        /// be performed instead by specifying the <c>get</c> parameter (note that <c>#</c> specifies the element itself, when used in <c>get</c>).
        /// Referring to the <a href="http://redis.io/commands/sort">redis SORT documentation </a> for examples is recommended. When used in hashes, <c>by</c> and <c>get</c>
        /// can be used to specify fields using <c>-&gt;</c> notation (again, refer to redis documentation).
        /// </summary>
        /// <remarks>http://redis.io/commands/sort</remarks>
        /// <returns>Returns the sorted elements, or the external values if <c>get</c> is specified</returns>
        Task<T[]> SortAsync(long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, T by = default(T), T[] get = null, CommandFlags flags = CommandFlags.None);
    }
}
