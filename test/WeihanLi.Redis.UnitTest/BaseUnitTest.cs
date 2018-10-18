namespace WeihanLi.Redis.UnitTest
{
    public class BaseUnitTest
    {
        static BaseUnitTest()
        {
            RedisManager.AddRedisConfig(config =>
            {
                config.CachePrefix = "WeihanLi.Redis.UnitTest";
                config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
                config.EnableCompress = false;
            });
        }
    }
}
