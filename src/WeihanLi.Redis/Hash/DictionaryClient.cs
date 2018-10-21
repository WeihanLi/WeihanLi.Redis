using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using WeihanLi.Redis.Internals;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Redis
{
    internal class DictionaryClient<TKey, TValue> : BaseRedisClient, IDictionaryClient<TKey, TValue>
    {
        private readonly string _realKey;
        private readonly bool _isSlidingExpired;
        private readonly TimeSpan? _expiry;
        private readonly DateTime? _expiresAt;

        /// <summary>
        /// 创建 DictionaryClient，默认滑动过期
        /// </summary>
        /// <param name="keyName">keyName</param>
        /// <param name="expiry">过期时间</param>
        public DictionaryClient(string keyName, TimeSpan? expiry, ILogger<DictionaryClient<TKey, TValue>> logger) : this(keyName, expiry, true, logger)
        {
        }

        /// <summary>
        /// 创建 DictionaryClient，绝对过期时间
        /// </summary>
        /// <param name="keyName">keyName</param>
        /// <param name="expiry">过期时间</param>
        public DictionaryClient(string keyName, DateTime? expiry, ILogger<DictionaryClient<TKey, TValue>> logger) : base(logger, new RedisWrapper(RedisConstants.DictionaryPrefix))
        {
            _realKey = Wrapper.GetRealKey(keyName);
            _isSlidingExpired = false;
            _expiresAt = expiry;
        }

        public DictionaryClient(string keyName, TimeSpan? expiry, bool isSlidingExpired, ILogger<DictionaryClient<TKey, TValue>> logger) : base(logger, new RedisWrapper(RedisConstants.DictionaryPrefix))
        {
            _realKey = Wrapper.GetRealKey(keyName);

            _isSlidingExpired = isSlidingExpired;
            if (isSlidingExpired)
            {
                _expiry = expiry;
            }
            else
            {
                _expiresAt = expiry.HasValue ? DateTime.Now.Add(expiry.Value) : (DateTime?)null;
            }
        }

        public bool Add(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None)
        {
            var result = Wrapper.Database.HashSet(_realKey, Wrapper.Wrap(fieldName), Wrapper.Wrap(value), When.NotExists, flags);
            if (result)
            {
                Expire(flags);
            }
            return result;
        }

        public async Task<bool> AddAsync(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None)
        {
            var result = await Wrapper.Database.HashSetAsync(_realKey, Wrapper.Wrap(fieldName), Wrapper.Wrap(value), When.NotExists, flags);
            if (result)
            {
                await ExpireAsync(flags);
            }
            return result;
        }

        public bool Add(IDictionary<TKey, TValue> values, CommandFlags flags = CommandFlags.None)
        {
            Wrapper.Database.HashSet(_realKey, values.Select(_ => new HashEntry(Wrapper.Wrap(_.Key), Wrapper.Wrap(_.Value))).ToArray(), flags);
            Expire(flags);
            return true;
        }

        public async Task<bool> AddAsync(IDictionary<TKey, TValue> values, CommandFlags flags = CommandFlags.None)
        {
            await Wrapper.Database
                .HashSetAsync(_realKey,
                    values.Select(_ => new HashEntry(Wrapper.Wrap(_.Key), Wrapper.Wrap(_.Value))).ToArray(), flags)
                .ContinueWith(_ => ExpireAsync(flags));
            return true;
        }

        public bool Clear(CommandFlags flags = CommandFlags.None) => Wrapper.Database.KeyDelete(_realKey, flags);

        public Task<bool> ClearAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Database.KeyDeleteAsync(_realKey, flags);

        public long Count(CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashLength(_realKey, flags);

        public Task<long> CountAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashLengthAsync(_realKey, flags);

        public bool Exists(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExists(_realKey, Wrapper.Wrap(fieldName), flags);

        public Task<bool> ExistsAsync(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Database.HashExistsAsync(_realKey, Wrapper.Wrap(fieldName), flags);

        public TValue Get(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TValue>(Wrapper.Database.HashGet(_realKey, Wrapper.Wrap(fieldName), flags));

        public async Task<TValue> GetAsync(TKey fieldName, CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TValue>(await Wrapper.Database.HashGetAsync(_realKey, Wrapper.Wrap(fieldName), flags));

        public TValue GetOrAdd(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None)
        {
            if (Exists(fieldName, flags))
            {
                return Get(fieldName, flags);
            }
            var result = Set(fieldName, value, When.NotExists, flags);
            return result ? value : Get(fieldName, flags);
        }

        public async Task<TValue> GetOrAddAsync(TKey fieldName, TValue value, CommandFlags flags = CommandFlags.None)
        {
            if (await ExistsAsync(fieldName, flags))
            {
                return await GetAsync(fieldName, flags);
            }
            var result = await SetAsync(fieldName, value, When.NotExists, flags);
            return result ? value : await GetAsync(fieldName, flags);
        }

        public TValue GetOrAdd(TKey fieldName, Func<TKey, TValue> func, CommandFlags flags = CommandFlags.None)
        {
            if (Exists(fieldName, flags))
            {
                return Get(fieldName, flags);
            }
            var value = func(fieldName);
            var result = Set(fieldName, value, When.NotExists, flags);
            return result ? value : Get(fieldName, flags);
        }

        public async Task<TValue> GetOrAddAsync(TKey fieldName, Func<TKey, Task<TValue>> func, CommandFlags flags = CommandFlags.None)
        {
            if (await ExistsAsync(fieldName, flags))
            {
                return await GetAsync(fieldName, flags);
            }

            var value = await func(fieldName);
            var result = await SetAsync(fieldName, value, When.NotExists, flags);
            return result ? value : await GetAsync(fieldName, flags);
        }

        public TKey[] Keys(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TKey>(Wrapper.Database.HashKeys(_realKey, flags));

        public async Task<TKey[]> KeysAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TKey>(await Wrapper.Database.HashKeysAsync(_realKey, flags));

        public bool Remove(TKey fieldName, CommandFlags flags = CommandFlags.None)
        {
            var result = Wrapper.Database.HashDelete(_realKey, Wrapper.Wrap(fieldName), flags);
            if (result)
            {
                Expire(flags, false);
            }
            return result;
        }

        public async Task<bool> RemoveAsync(TKey fieldName, CommandFlags flags = CommandFlags.None)
        {
            var result = await Wrapper.Database.HashDeleteAsync(_realKey, Wrapper.Wrap(fieldName), flags);
            if (result)
            {
                await ExpireAsync(flags, false);
            }
            return result;
        }

        public bool Set(TKey fieldName, TValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var result = Wrapper.Database.HashSet(_realKey, Wrapper.Wrap(fieldName), Wrapper.Wrap(value), when, flags);
            if (result)
            {
                Expire(flags);
            }
            return result;
        }

        public async Task<bool> SetAsync(TKey fieldName, TValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var result = await Wrapper.Database.HashSetAsync(_realKey, Wrapper.Wrap(fieldName), Wrapper.Wrap(value), when, flags);
            if (result)
            {
                await ExpireAsync(flags);
            }
            return result;
        }

        public TValue[] Values(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TValue>(Wrapper.Database.HashValues(_realKey, flags));

        public async Task<TValue[]> ValuesAsync(CommandFlags flags = CommandFlags.None) => Wrapper.Unwrap<TValue>(await Wrapper.Database.HashValuesAsync(_realKey, flags));

        #region Expire

        private void Expire(CommandFlags flags) => Expire(flags, true);

        private void Expire(CommandFlags flags, bool isSet)
        {
            if (_isSlidingExpired && _expiry.HasValue)
            {
                Wrapper.Database.KeyExpire(_realKey, _expiry, flags);
            }
            else
            {
                if (isSet && _expiresAt.HasValue && null == Wrapper.Database.KeyTimeToLive(_realKey, flags))
                {
                    Wrapper.Database.KeyExpire(_realKey, _expiresAt, flags);
                }
            }
        }

        private Task ExpireAsync(CommandFlags flags) => ExpireAsync(flags, true);

        private async Task ExpireAsync(CommandFlags flags, bool isSet)
        {
            if (_isSlidingExpired && _expiry.HasValue)
            {
                await Wrapper.Database.KeyExpireAsync(_realKey, _expiry, flags);
            }
            else
            {
                if (isSet && _expiresAt.HasValue && null == Wrapper.Database.KeyTimeToLive(_realKey, flags))
                {
                    await Wrapper.Database.KeyExpireAsync(_realKey, _expiresAt, flags);
                }
            }
        }

        #endregion Expire
    }
}
