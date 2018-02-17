using System;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Redis
{
    internal class HashCounterClient : BaseRedisClient, ICounterClient
    {
        private const string KeyName = "Counter";
        private readonly string _fieldName;

        private readonly TimeSpan? _expiresIn;

        public HashCounterClient(string counterName, long baseCount, TimeSpan? expiresIn) : base(LogHelper.GetLogHelper<CounterClient>(), new RedisWrapper("Hash/Counter"))
        {
            _fieldName = $"Hash/Counter/{counterName}";
            Base = baseCount;
            _expiresIn = expiresIn;
            Reset();
        }

        public HashCounterClient(string counterName, TimeSpan? expiresIn) : this(counterName, 0, expiresIn)
        {
        }

        public HashCounterClient(string counterName, long baseCount) : this(counterName, baseCount, null)
        {
        }

        public HashCounterClient(string counterName) : this(counterName, 0, null)
        {
        }

        public long Count => Wrapper.Wrap<int>(_fieldName, k => Wrapper.Database.HashGet(KeyName, _fieldName));

        public long Base { get; }

        public long Decrease() => Decrease(1);

        public long Decrease(int step) => Wrapper.Database.HashDecrement(KeyName, _fieldName, step);

        public long Increase() => Increase(1);

        public long Increase(int step) => Wrapper.Database.HashIncrement(KeyName, _fieldName, step);

        public bool Reset()
        {
            var result = Wrapper.Database.HashSet(KeyName, _fieldName, Base);
            if (result)
            {
                Wrapper.Database.KeyExpire($"{KeyName}:{_fieldName}", _expiresIn);
            }
            return result;
        }

        public async Task<bool> ResetAsync()
        {
            var result = await Wrapper.Database.HashSetAsync(KeyName, _fieldName, Base);
            if (result)
            {
                await Wrapper.Database.KeyExpireAsync($"{KeyName}:{_fieldName}", _expiresIn);
            }
            return result;
        }

        public Task<long> IncreaseAsync() => IncreaseAsync(1);

        public Task<long> DecreaseAsync() => DecreaseAsync(1);

        public Task<long> IncreaseAsync(int step) => Wrapper.Database.HashIncrementAsync(KeyName, _fieldName, step);

        public Task<long> DecreaseAsync(int step) => Wrapper.Database.HashDecrementAsync(KeyName, _fieldName, step);
    }
}
