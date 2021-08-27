using StackExchange.Redis;
using System.Threading.Tasks;

namespace WeihanLi.Redis
{
    public static class RedisExtensions
    {
        /// <summary>
        /// Hash Compare And Exchange
        /// </summary>
        private const string HashCasLuaScript = @"
if redis.call(""HGET"", KEYS[1], ARGV[1]) == ARGV[2] then
    redis.call(""HSET"", KEYS[1], ARGV[1], ARGV[3])
    return 1
else
    return 0
end
";

        /// <summary>
        /// Hash Compare And Delete
        /// </summary>
        private const string HashCadLuaScript = @"
if redis.call(""HGET"", KEYS[1], ARGV[1]) == ARGV[2] then
    redis.call(""HDEL"", KEYS[1], ARGV[1])
    return 1
else
    return 0
end
";

        /// <summary>
        /// Hash Set Only When Value changed
        /// </summary>
        private const string HashSetWhenValueChangedLuaScript = @"
if redis.call(""HGET"", KEYS[1], ARGV[1]) == ARGV[2] then
    return 0
else
    redis.call(""HSET"", KEYS[1], ARGV[1], ARGV[2])
    return 1
end
";

        /// <summary>
        /// String Compare And Exchange
        /// </summary>
        private const string StringCasLuaScript = @"
if redis.call(""GET"", KEYS[1]) == ARGV[1] then
    redis.call(""SET"", KEYS[1], ARGV[2])
    return 1
else
    return 0
end
";

        /// <summary>
        /// String Compare And Delete
        /// </summary>
        private const string StringCadScript = @"
if redis.call(""GET"",KEYS[1]) == ARGV[1] then
    return redis.call(""DEL"",KEYS[1])
else
    return 0
end";

        /// <summary>
        /// String Set only when value changed
        /// </summary>
        private const string StringSetWhenValueChangedLuaScript = @"
if redis.call(""GET"", KEYS[1]) == ARGV[1] then
    return 0
else
    redis.call(""SET"", KEYS[1], ARGV[1])
    return 1
end
";

        public static bool StringCompareAndDelete(this IDatabase db, RedisKey key, RedisValue originValue)
        {
            return (int)db.ScriptEvaluate(StringCadScript, new[] { key }, new[] { originValue }) == 1;
        }

        public static async Task<bool> StringCompareAndDeleteAsync(this IDatabase db, RedisKey key, RedisValue originValue)
        {
            return await db.ScriptEvaluateAsync(StringCadScript, new[] { key }, new[] { originValue })
                .ContinueWith(r => (int)r.Result == 1);
        }

        public static bool StringSetWhenValueChanged(this IDatabase db, RedisKey key, RedisValue value)
        {
            return (int)db.ScriptEvaluate(StringSetWhenValueChangedLuaScript, new[] { key }, new[] { value }) == 1;
        }

        public static async Task<bool> StringSetWhenValueChangedAsync(this IDatabase db, RedisKey key, RedisValue value)
        {
            return await db.ScriptEvaluateAsync(StringSetWhenValueChangedLuaScript, new[] { key }, new[] { value })
                .ContinueWith(r => (int)r.Result == 1);
        }

        public static bool StringCompareAndExchange(this IDatabase db, RedisKey key, RedisValue newValue, RedisValue originValue)
        {
            return (int)db.ScriptEvaluate(StringCasLuaScript, new[] { key }, new[] { originValue, newValue }) == 1;
        }

        public static async Task<bool> StringCompareAndExchangeAsync(this IDatabase db, RedisKey key, RedisValue newValue, RedisValue originValue)
        {
            return await db.ScriptEvaluateAsync(StringCasLuaScript, new[] { key }, new[] { originValue, newValue })
                .ContinueWith(r => (int)r.Result == 1);
        }

        public static bool HashCompareAndDelete(this IDatabase db, RedisKey key, RedisValue field, RedisValue originValue)
        {
            return (int)db.ScriptEvaluate(HashCadLuaScript, new[] { key }, new[] { field, originValue }) == 1;
        }

        public static async Task<bool> HashCompareAndDeleteAsync(this IDatabase db, RedisKey key, RedisValue field, RedisValue originValue)
        {
            return await db.ScriptEvaluateAsync(HashCadLuaScript, new[] { key }, new[] { field, originValue })
                .ContinueWith(r => (int)r.Result == 1);
        }

        public static bool HashSetWhenValueChanged(this IDatabase db, RedisKey key, RedisValue field, RedisValue value)
        {
            return (int)db.ScriptEvaluate(HashSetWhenValueChangedLuaScript, new[] { key }, new[] { field, value }) == 1;
        }

        public static async Task<bool> HashSetWhenValueChangedAsync(this IDatabase db, RedisKey key, RedisValue field, RedisValue value)
        {
            return await db.ScriptEvaluateAsync(HashSetWhenValueChangedLuaScript, new[] { key }, new[] { field, value })
                .ContinueWith(r => (int)r.Result == 1);
        }

        public static bool HashCompareAndExchange(this IDatabase db, RedisKey key, RedisValue field, RedisValue newValue, RedisValue originValue)
        {
            return (int)db.ScriptEvaluate(HashCasLuaScript, new[] { key }, new[] { field, originValue, newValue }) == 1;
        }

        public static async Task<bool> HashCompareAndExchangeAsync(this IDatabase db, RedisKey key, RedisValue field, RedisValue newValue, RedisValue originValue)
        {
            return await db.ScriptEvaluateAsync(HashCasLuaScript, new[] { key }, new[] { field, originValue, newValue })
                .ContinueWith(r => (int)r.Result == 1);
        }
    }
}
