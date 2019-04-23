using System.Threading.Tasks;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    /// <summary>
    /// HashCounter
    /// </summary>
    public interface IHashCounterClient : IRedisClient
    {
        long Base { get; }

        long Count(RedisValue field, CommandFlags flags = CommandFlags.None);

        bool Reset(RedisValue field, CommandFlags flags = CommandFlags.None);

        Task<bool> ResetAsync(RedisValue field, CommandFlags flags = CommandFlags.None);

        long Increase(RedisValue field, CommandFlags flags = CommandFlags.None);

        long Decrease(RedisValue field, CommandFlags flags = CommandFlags.None);

        long Increase(RedisValue field, int step, CommandFlags flags = CommandFlags.None);

        long Decrease(RedisValue field, int step, CommandFlags flags = CommandFlags.None);

        Task<long> IncreaseAsync(RedisValue field, CommandFlags flags = CommandFlags.None);

        Task<long> DecreaseAsync(RedisValue field, CommandFlags flags = CommandFlags.None);

        Task<long> IncreaseAsync(RedisValue field, int step, CommandFlags flags = CommandFlags.None);

        Task<long> DecreaseAsync(RedisValue field, int step, CommandFlags flags = CommandFlags.None);
    }
}
