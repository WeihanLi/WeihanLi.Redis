using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface IListClient<T> : IRedisClient
    {
        //
        // 摘要:
        //     Returns the element at index index in the list stored at key. The index is zero-based,
        //     so 0 means the first element, 1 the second element and so on. Negative indices
        //     can be used to designate elements starting at the tail of the list. Here, -1
        //     means the last element, -2 means the penultimate and so forth.
        //
        // 返回结果:
        //     the requested element, or nil when index is out of range.
        //
        // 备注:
        //     http://redis.io/commands/lindex
        T Get(long index, CommandFlags flags = CommandFlags.None);

        Task<T> GetAsync(long index, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Inserts value in the list stored at key either before or after the reference
        //     value pivot. When key does not exist, it is considered an empty list and no operation
        //     is performed.
        //
        // 返回结果:
        //     the length of the list after the insert operation, or -1 when the value pivot
        //     was not found.
        //
        // 备注:
        //     http://redis.io/commands/linsert
        long InsertAfter(T pivot, T value, CommandFlags flags = CommandFlags.None);

        Task<long> InsertAfterAsync(T pivot, T value, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Inserts value in the list stored at key either before or after the reference
        //     value pivot. When key does not exist, it is considered an empty list and no operation
        //     is performed.
        //
        // 返回结果:
        //     the length of the list after the insert operation, or -1 when the value pivot
        //     was not found.
        //
        // 备注:
        //     http://redis.io/commands/linsert
        long InsertBefore(T pivot, T value, CommandFlags flags = CommandFlags.None);

        Task<long> InsertBeforeAsync(T pivot, T value, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Removes and returns the first element of the list stored at key.
        //
        // 返回结果:
        //     the value of the first element, or nil when key does not exist.
        //
        // 备注:
        //     http://redis.io/commands/lpop
        T LeftPop(CommandFlags flags = CommandFlags.None);

        Task<T> LeftPopAsync(CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Insert all the specified values at the head of the list stored at key. If key
        //     does not exist, it is created as empty list before performing the push operations.
        //     Elements are inserted one after the other to the head of the list, from the leftmost
        //     element to the rightmost element. So for instance the command LPUSH mylist a
        //     b c will result into a list containing c as first element, b as second element
        //     and a as third element.
        //
        // 返回结果:
        //     the length of the list after the push operations.
        //
        // 备注:
        //     http://redis.io/commands/lpush
        long LeftPush(T[] values, CommandFlags flags = CommandFlags.None);

        Task<long> LeftPushAsync(T[] values, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Insert the specified value at the head of the list stored at key. If key does
        //     not exist, it is created as empty list before performing the push operations.
        //
        // 返回结果:
        //     the length of the list after the push operations.
        //
        // 备注:
        //     http://redis.io/commands/lpush
        long LeftPush(T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        Task<long> LeftPushAsync(T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Returns the length of the list stored at key. If key does not exist, it is interpreted
        //     as an empty list and 0 is returned.
        //
        // 返回结果:
        //     the length of the list at key.
        //
        // 备注:
        //     http://redis.io/commands/llen
        long Count(CommandFlags flags = CommandFlags.None);

        Task<long> CountAsync(CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Returns the specified elements of the list stored at key. The offsets start and
        //     stop are zero-based indexes, with 0 being the first element of the list (the
        //     head of the list), 1 being the next element and so on. These offsets can also
        //     be negative numbers indicating offsets starting at the end of the list.For example,
        //     -1 is the last element of the list, -2 the penultimate, and so on. Note that
        //     if you have a list of numbers from 0 to 100, LRANGE list 0 10 will return 11
        //     elements, that is, the rightmost item is included.
        //
        // 返回结果:
        //     list of elements in the specified range.
        //
        // 备注:
        //     http://redis.io/commands/lrange
        T[] ListRange(long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None);

        Task<T[]> ListRangeAsync(long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Removes the first count occurrences of elements equal to value from the list
        //     stored at key. The count argument influences the operation in the following ways:
        //     count > 0: Remove elements equal to value moving from head to tail. count < 0:
        //     Remove elements equal to value moving from tail to head. count = 0: Remove all
        //     elements equal to value.
        //
        // 返回结果:
        //     the number of removed elements.
        //
        // 备注:
        //     http://redis.io/commands/lrem
        long Remove(T value, long count = 0, CommandFlags flags = CommandFlags.None);

        Task<long> RemoveAsync(T value, long count = 0, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Removes and returns the last element of the list stored at key.
        //
        // 备注:
        //     http://redis.io/commands/rpop
        T Pop(CommandFlags flags = CommandFlags.None);

        Task<T> PopAsync(CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Atomically returns and removes the last element (tail) of the list stored at
        //     source, and pushes the element at the first element (head) of the list stored
        //     at destination.
        //
        // 返回结果:
        //     the element being popped and pushed.
        //
        // 备注:
        //     http://redis.io/commands/rpoplpush
        T RightPopLeftPush(string destination, CommandFlags flags = CommandFlags.None);

        Task<T> RightPopLeftPushAsync(string destination, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Insert the specified value at the tail of the list stored at key. If key does
        //     not exist, it is created as empty list before performing the push operation.
        //
        // 返回结果:
        //     the length of the list after the push operation.
        //
        // 备注:
        //     http://redis.io/commands/rpush
        long Push(T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        Task<long> PushAsync(T value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Insert all the specified values at the tail of the list stored at key. If key
        //     does not exist, it is created as empty list before performing the push operation.
        //     Elements are inserted one after the other to the tail of the list, from the leftmost
        //     element to the rightmost element. So for instance the command RPUSH mylist a
        //     b c will result into a list containing a as first element, b as second element
        //     and c as third element.
        //
        // 返回结果:
        //     the length of the list after the push operation.
        //
        // 备注:
        //     http://redis.io/commands/rpush
        long Push(T[] values, CommandFlags flags = CommandFlags.None);

        Task<long> PushAsync(T[] values, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Sets the list element at index to value. For more information on the index argument,
        //     see ListGetByIndex. An error is returned for out of range indexes.
        //
        // 备注:
        //     http://redis.io/commands/lset
        bool Set(long index, T value, CommandFlags flags = CommandFlags.None);

        Task<bool> SetAsync(long index, T value, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Trim an existing list so that it will contain only the specified range of elements
        //     specified. Both start and stop are zero-based indexes, where 0 is the first element
        //     of the list (the head), 1 the next element and so on. For example: LTRIM foobar
        //     0 2 will modify the list stored at foobar so that only the first three elements
        //     of the list will remain. start and end can also be negative numbers indicating
        //     offsets from the end of the list, where -1 is the last element of the list, -2
        //     the penultimate element and so on.
        //
        // 备注:
        //     http://redis.io/commands/ltrim
        bool Trim(long start, long stop, CommandFlags flags = CommandFlags.None);

        Task<bool> TrimAsync(long start, long stop, CommandFlags flags = CommandFlags.None);

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
    }
}
