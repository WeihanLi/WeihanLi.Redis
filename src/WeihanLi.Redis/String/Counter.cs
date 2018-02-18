// ReSharper disable once CheckNamespace
using System;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    public interface ICounterClient
    {
        long Count { get; }

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

        public CounterClient(string counterName, long baseCount, TimeSpan? expiresIn) : base(LogHelper.GetLogHelper<CounterClient>(), new RedisWrapper("String/Counter/"))
        {
            _keyName = $"String/Counter/{counterName}";
            Base = baseCount;
            _expiresIn = expiresIn;
            Reset();
        }

        public CounterClient(string counterName, TimeSpan? expiresIn) : this(counterName, 0, expiresIn)
        {
        }

        public CounterClient(string counterName, long baseCount) : this(counterName, baseCount, null)
        {
        }

        public CounterClient(string counterName) : this(counterName, 0, null)
        {
        }

        public long Count => Wrapper.Wrap<int>(_keyName, k => Wrapper.Database.StringGet(_keyName));

        public long Base { get; }

        public long Decrease() => Decrease(1);

        public long Decrease(int step) => Wrapper.Database.StringDecrement(_keyName, step);

        public long Increase() => Increase(1);

        public long Increase(int step) => Wrapper.Database.StringIncrement(_keyName, step);

        public bool Reset() => Wrapper.Database.StringSet(_keyName, Base, _expiresIn);

        public Task<bool> ResetAsync() => Wrapper.Database.StringSetAsync(_keyName, Base, _expiresIn);

        public Task<long> IncreaseAsync() => IncreaseAsync(1);

        public Task<long> DecreaseAsync() => DecreaseAsync(1);

        public Task<long> IncreaseAsync(int step) => Wrapper.Database.StringIncrementAsync(_keyName, step);

        public Task<long> DecreaseAsync(int step) => Wrapper.Database.StringDecrementAsync(_keyName, step);
    }
}
