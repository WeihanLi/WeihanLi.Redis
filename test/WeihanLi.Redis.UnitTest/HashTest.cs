using System;
using Xunit;

namespace WeihanLi.Redis.UnitTest
{
    public class HashTest : BaseUnitTest
    {
        [Fact]
        public void HashCacheTest()
        {
            var key = "hashTest";
            var fieldName = "testField1";
            var value = "Hello WeihanLi.Redis";
            var hashClient = RedisManager.GetHashClient();
            var result = hashClient.Set(key, fieldName, value);
            Assert.True(result);
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
            Assert.Equal(2, dictionary.Count());
            Assert.False(dictionary.Exists("chinese"));
            Assert.True(dictionary.Remove("English"));
        }
    }
}
