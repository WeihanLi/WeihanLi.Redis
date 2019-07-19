using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class HashTest : BaseUnitTest
    {
        [Fact]
        public void HashCounterTest()
        {
            var key = "hashTest";
            var fieldName = "testField1";
            var hashClient = RedisManager.GetHashCounterClient(key);
            var commonClient = RedisManager.GetCommonRedisClient(RedisDataType.HashCounter);

            var result = hashClient.Increase(fieldName);
            Assert.True(1 == result);
            result = hashClient.Increase(fieldName, 12);
            Assert.True(13 == result);
            result = hashClient.Decrease(fieldName, 2);
            Assert.True(11 == result);
            result = hashClient.Decrease(fieldName);
            Assert.True(10 == result);
            Assert.True(commonClient.KeyDelete(key));
            Assert.False(commonClient.KeyExists(key));
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(50)]
        public async Task CounterConcurrentTest(int taskCount)
        {
            var counterName = "concurrentCounterTest";
            var fieldName = $"testField1_{taskCount}";

            RedisManager.GetCommonRedisClient(RedisDataType.Counter).KeyDelete(counterName);
            try
            {
                var tasks = new List<Task>();
                Func<Task> func = () =>
                {
                    var counter = RedisManager.GetHashCounterClient(counterName);
                    return counter.IncreaseAsync(fieldName);
                };
                for (var i = 0; i < taskCount; i++)
                {
                    tasks.Add(func());
                }
                await Task.WhenAll(tasks);

                Assert.Equal(taskCount, RedisManager.GetHashCounterClient(counterName).Count(fieldName));
            }
            finally
            {
                RedisManager.GetCommonRedisClient(RedisDataType.Counter).KeyDelete(counterName);
            }
        }

        [Fact]
        public void HashCacheTest()
        {
            var key = "hashTest";
            var fieldName = "testField1";
            var value = "Hello WeihanLi.Redis";
            var hashClient = RedisManager.HashClient;
            var result = hashClient.Set(key, fieldName, value);
            Assert.True(result);
            hashClient.Set(key, fieldName + "1", value);
            var vals = hashClient.Get(key, new RedisValue[] { fieldName, "gfhjkghjgh", fieldName + "1" });
            Assert.Equal(3, vals.Length);
            Assert.Null(vals[1]);
            Assert.True(hashClient.Expire(key, TimeSpan.FromSeconds(10)));
            Assert.True(hashClient.Exists(key, fieldName));
            Assert.Equal(value, hashClient.Get(key, fieldName));
            Assert.True(hashClient.Remove(key, fieldName));
            Assert.False(hashClient.Exists(key, fieldName));
        }

        [Fact]
        public void DictionaryTest()
        {
            var dictionary = RedisManager.GetDictionaryClient<string, int>("testDictionary", TimeSpan.FromMinutes(1));
            Assert.Equal(0, dictionary.Count());
            Assert.False(dictionary.Exists("chinese"));
            dictionary.Add("math", 80);
            dictionary.Add("English", 60);
            Assert.True(dictionary.Exists("math"));
            Assert.Equal(80, dictionary.GetOrAdd("math", 20));
            Assert.Equal(2, dictionary.Count());
            Assert.False(dictionary.Exists("chinese"));
            Assert.True(dictionary.Remove("English"));
        }
    }
}
