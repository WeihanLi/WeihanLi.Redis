using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface IStackClient<T> : IRedisClient
    {
        //
        // 摘要:
        //     Removes and returns the last element of the list stored at key.
        //
        // 备注:
        //     http://redis.io/commands/rpop
        T Pop(CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Removes and returns the last element of the list stored at key.
        //
        // 备注:
        //     http://redis.io/commands/rpop
        Task<T> PopAsync(CommandFlags flags = CommandFlags.None);

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
        Task<long> PushAsync(T[] values, CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Returns the length of the list stored at key. If key does not exist, it is interpreted
        //     as an empty list and 0 is returned.
        //
        // 返回结果:
        //     the length of the queue
        //
        // 备注:
        //     http://redis.io/commands/llen
        long Length(CommandFlags flags = CommandFlags.None);

        //
        // 摘要:
        //     Returns the length of the list stored at key. If key does not exist, it is interpreted
        //     as an empty list and 0 is returned.
        //
        // 返回结果:
        //     the length of the queue
        //
        // 备注:
        //     http://redis.io/commands/llen
        Task<long> LengthAsync(CommandFlags flags = CommandFlags.None);

        T this[long index] { get; set; }
    }
}
