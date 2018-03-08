using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class DictionaryClient<TKey, TValue> : BaseRedisClient, IDictionaryClient<TKey, TValue>
    {
        private readonly string _realKey;
        private readonly bool _isSlidingExpired;
        private readonly TimeSpan? _expiry;

        /// <summary>
        /// 创建 DictionaryClient，默认滑动过期
        /// </summary>
        /// <param name="keyName">keyName</param>
        /// <param name="expiry">过期时间</param>
        public DictionaryClient(string keyName, TimeSpan? expiry) : this(keyName, expiry, true)
        {
        }

        public DictionaryClient(string keyName, TimeSpan? expiry, bool isSlidingExpired) : base(LogHelper.GetLogHelper<DictionaryClient<TKey, TValue>>(), new RedisWrapper("Hash/Dictionary"))
        {
            _realKey = Wrapper.GetRealKey(keyName);
            _expiry = expiry;
            _isSlidingExpired = isSlidingExpired;
        }

        public bool Add(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None)
        {
            var result = Wrapper.Database.HashSet(_realKey, Wrapper.Wrap(fieldName), Wrapper.Wrap(value), When.NotExists, flags);
            if (result && _isSlidingExpired)
            {
                Wrapper.Database.KeyExpire(_realKey, _expiry, flags);
            }
            return result;
        }

        public async Task<bool> AddAsync(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None)
        {
            var result = await Wrapper.Database.HashSetAsync(_realKey, Wrapper.Wrap(fieldName), Wrapper.Wrap(value), When.NotExists, flags);
            if (result && _isSlidingExpired)
            {
                await Wrapper.Database.KeyExpireAsync(_realKey, _expiry, flags).ConfigureAwait(false);
            }
            return result;
        }

        public long Count(CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashLength(_realKey, flags);

        public Task<long> CountAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashLengthAsync(_realKey, flags);

        public bool Exists(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExists(_realKey, Wrapper.Wrap(fieldName), flags);

        public Task<bool> ExistsAsync(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExistsAsync(_realKey, Wrapper.Wrap(fieldName), flags);

        public TValue Get(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TValue>(Wrapper.Database.HashGet(_realKey, Wrapper.Wrap(fieldName), flags));

        public async Task<TValue> GetAsync(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TValue>(await Wrapper.Database.HashGetAsync(_realKey, Wrapper.Wrap(fieldName), flags));

        public TKey[] Keys(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TKey>(Wrapper.Database.HashValues(_realKey, flags));

        public async Task<TKey[]> KeysAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TKey>(await Wrapper.Database.HashValuesAsync(_realKey, flags));

        public bool Remove(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDelete(_realKey, Wrapper.Wrap(fieldName), flags);

        public Task<bool> RemoveAsync(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashDeleteAsync(_realKey, Wrapper.Wrap(fieldName), flags);

        public bool Set(TKey fieldName, TValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var result = Wrapper.Database.HashSet(_realKey, Wrapper.Wrap(fieldName), Wrapper.Wrap(value), when, flags);
            if (result && _isSlidingExpired)
            {
                Wrapper.Database.KeyExpire(_realKey, _expiry, flags);
            }
            return result;
        }

        public async Task<bool> SetAsync(TKey fieldName, TValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var result = await Wrapper.Database.HashSetAsync(_realKey, Wrapper.Wrap(fieldName), Wrapper.Wrap(value), when, flags);
            if (result && _isSlidingExpired)
            {
                await Wrapper.Database.KeyExpireAsync(_realKey, _expiry, flags).ConfigureAwait(false);
            }
            return result;
        }

        public TValue[] Values<T>(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TValue>(Wrapper.Database.HashValues(_realKey, flags));

        public async Task<TValue[]> ValuesAsync<T>(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TValue>(await Wrapper.Database.HashValuesAsync(_realKey, flags));
    }
}
