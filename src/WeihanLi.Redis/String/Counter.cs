using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    public interface ICounterClient : IRedisClient
    {
        long Count();

        long Base { get; }

        bool Reset();

        Task<bool> ResetAsync();

        long Increase();

        long Decrease();

        long Increase(int step);

        long Decrease(int step);

        Task<long> IncreaseAsync();

        Task<long> DecreaseAsync();

        Task<long> IncreaseAsync(int step);

        Task<long> DecreaseAsync(int step);
    }

    internal class CounterClient : BaseRedisClient, ICounterClient
    {
        private readonly string _keyName;

        private readonly TimeSpan? _expiresIn;

        public CounterClient(string counterName, long baseCount, TimeSpan? expiresIn, ILogger<CounterClient> logger) : base(logger, new RedisWrapper(RedisConstants.CounterPrefix))
        {
            _keyName = Wrapper.GetRealKey(counterName);
            Base = baseCount;
            _expiresIn = expiresIn;
        }

        public CounterClient(string counterName, TimeSpan? expiresIn, ILogger<CounterClient> logger) : this(counterName, 0, expiresIn, logger)
        {
        }

        public CounterClient(string counterName, long baseCount, ILogger<CounterClient> logger) : this(counterName, baseCount, null, logger)
        {
        }

        public CounterClient(string counterName, ILogger<CounterClient> logger) : this(counterName, 0, null, logger)
        {
        }

        private void SetExpiryIfNeed()
        {
            if (_expiresIn.HasValue)
            {
                Wrapper.Database.StringSet(_keyName, Base, _expiresIn, When.NotExists);
            }
        }

        private Task SetExpiryIfNeedAsync()
        {
            if (_expiresIn.HasValue)
            {
                return Wrapper.Database.StringSetAsync(_keyName, Base, _expiresIn, When.NotExists);
            }
            return Task.CompletedTask;
        }

        public long Count() => Convert.ToInt64(Wrapper.Database.StringGet(_keyName));

        public long Base { get; }

        public long Decrease() => Decrease(1);

        public long Decrease(int step)
        {
            SetExpiryIfNeed();
            return Wrapper.Database.StringDecrement(_keyName, step);
        }

        public long Increase() => Increase(1);

        public long Increase(int step)
        {
            SetExpiryIfNeed();
            return Wrapper.Database.StringIncrement(_keyName, step);
        }

        public bool Reset() => Wrapper.Database.StringSet(_keyName, Base, _expiresIn);

        public Task<bool> ResetAsync() => Wrapper.Database.StringSetAsync(_keyName, Base, _expiresIn);

        public Task<long> IncreaseAsync() => IncreaseAsync(1);

        public Task<long> DecreaseAsync() => DecreaseAsync(1);

        public async Task<long> IncreaseAsync(int step)
        {
            await SetExpiryIfNeedAsync();
            return await Wrapper.Database.StringIncrementAsync(_keyName, step);
        }

        public async Task<long> DecreaseAsync(int step)
        {
            await SetExpiryIfNeedAsync();
            return await Wrapper.Database.StringDecrementAsync(_keyName, step);
        }
    }
}
