using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class HashCounterClient : BaseRedisClient, IHashCounterClient
    {
        private readonly string _realKey;

        public HashCounterClient(ILogger<HashCounterClient> logger, IRedisWrapper redisWrapper, string key) : this(logger, redisWrapper, key, 0)
        {
        }

        public HashCounterClient(ILogger<HashCounterClient> logger, IRedisWrapper redisWrapper, string key, long @base) : base(logger, redisWrapper)
        {
            _realKey = Wrapper.GetRealKey(key);
            Base = @base;
        }

        public long Base { get; }

        public long Count(RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            return long.TryParse(Wrapper.Database.Value.HashGet(_realKey, field, flags).ToString(), out var count)
                ? count
                : 0L;
        }

        public bool Reset(RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.Value.HashSet(_realKey, field, Base, flags: flags);
        }

        public Task<bool> ResetAsync(RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.Value.HashSetAsync(_realKey, field, Base, flags: flags);
        }

        public long Increase(RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            return Increase(field, 1, flags);
        }

        public long Decrease(RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            return Decrease(field, 1, flags);
        }

        public long Increase(RedisValue field, int step, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.Value.HashIncrement(_realKey, field, step, flags);
        }

        public long Decrease(RedisValue field, int step, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.Value.HashDecrement(_realKey, field, step, flags);
        }

        public Task<long> IncreaseAsync(RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            return IncreaseAsync(field, 1, flags);
        }

        public Task<long> DecreaseAsync(RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            return DecreaseAsync(field, 1, flags);
        }

        public Task<long> IncreaseAsync(RedisValue field, int step, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.Value.HashIncrementAsync(_realKey, field, step, flags);
        }

        public Task<long> DecreaseAsync(RedisValue field, int step, CommandFlags flags = CommandFlags.None)
        {
            return Wrapper.Database.Value.HashDecrementAsync(_realKey, field, step, flags);
        }
    }
}
