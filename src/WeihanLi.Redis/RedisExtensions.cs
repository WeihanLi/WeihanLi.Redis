using StackExchange.Redis;
using System.Threading.Tasks;

namespace WeihanLi.Redis
{
    public static class RedisExtensions
    {
        private const string HashCasLuaScript = @"
if redis.call(""hget"", KEYS[1], ARGV[1]) == ARGV[2] then
    redis.call(""hset"", KEYS[1], ARGV[1], ARGV[3])
    return 1
else
    return 0
end
";

        private const string StringCasLuaScript = @"
if redis.call(""get"", KEYS[1]) == ARGV[1] then
    redis.call(""set"", KEYS[1], ARGV[2])
    return 1
else
    return 0
end
";

        public static bool StringCompareAndExchange(this IDatabase db, RedisKey key, RedisValue newValue, RedisValue originValue)
        {
            return (int)db.ScriptEvaluate(StringCasLuaScript, new[] { key }, new[] { originValue, newValue }) == 1;
        }

        public static async Task<bool> StringCompareAndExchangeAsync(this IDatabase db, RedisKey key, RedisValue newValue, RedisValue originValue)
        {
            return await db.ScriptEvaluateAsync(StringCasLuaScript, new[] { key }, new[] { originValue, newValue })
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
