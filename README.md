# WeihanLi.Redis

## Intro

RedisExtensions for StackExchange.Redis, much more easier for generic opeartions,and supply some extensions for your business logics.

StackExchange.Redis 扩展，更简单的泛型操作，并提供一些的适用于业务场景中的扩展

基于 Redis 的五种数据类型扩展出了一些应用：

1. String

    - Cache
    - Counter
    - Firewall
    - RedLock

1. Hash

    - Hash
    - Dictonary

1. List

    - List

1. Set

    - Set

1. SortedSet

    - SortedSet
    - Rank

## GetStarted

### Installation 安装

Install from [Nuget](https://www.nuget.org/packages/WeihanLi.Redis/)

通过 Nuget 安装 `WeihanLi.Redis`

### Configuration 配置

``` csharp
RedisManager.AddRedisConfig(config =>
    {
        config.CachePrefix = "WeihanLi.Redis.UnitTest";
        config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
    });
```

.net core 应用，还可以这样配置

``` csharp
serviceCollection.AddRedisConfig(config =>
    {
        config.CachePrefix = "WeihanLi.Redis.UnitTest";
        config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
        config.EnableCompress = false;// disable compress
    });
```

### Basic usage 基本用法

1. Cache 缓存

    缓存的基本操作主要是基于 `RedisManager.CacheClient`

    缓存的基本操作定义在 `ICacheClient` 中，基本操作如下：

    ``` csharp
    var key = "test111";
    var value = "Hello WeihanLi.Redis";
    Assert.True(RedisManager.CacheClient.Set(key, value));
    Assert.True(RedisManager.CacheClient.Exists(key));
    Assert.Equal(value, RedisManager.CacheClient.Get(key));
    Assert.True(RedisManager.CacheClient.Remove(key));
    Assert.False(RedisManager.CacheClient.Exists(key));
    RedisManager.CacheClient.GetOrSet(key, () => value, TimeSpan.FromSeconds(10));
    ```

1. Counter 计数器

    ``` csharp
    var counterName = "counterTest";
    var counterClient = RedisManager.GetCounterClient(counterName, TimeSpan.FromSeconds(60));
    Assert.Equal(0, counterClient.Base);
    Assert.Equal(0, counterClient.Count());
    counterClient.Increase();
    Assert.Equal(1, counterClient.Count());
    counterClient.Increase(5);
    Assert.Equal(6, counterClient.Count());
    counterClient.Decrease(3);
    Assert.Equal(3, counterClient.Count());
    Assert.True(counterClient.Reset());
    Assert.Equal(0, counterClient.Count());
    ```

1. Firewall 防火墙

    ``` csharp
    var firewallName = "firewallTest";
    var firewallClient = RedisManager.GetFirewallClient(firewallName, TimeSpan.FromSeconds(3));
    Assert.True(firewallClient.Hit());
    Assert.False(firewallClient.Hit());
    await Task.Delay(TimeSpan.FromSeconds(3));
    Assert.True(firewallClient.Hit());
    ```

1. RedLock Redis分布式锁

    ``` csharp
    using (var client = RedisManager.GetRedLockClient("redLockTest"))
    {
        Assert.True(client.TryLock(TimeSpan.FromSeconds(10)));
        using (var client1 = RedisManager.GetRedLockClient("redLockTest"))
        {
            Assert.False(client.TryLock(TimeSpan.FromSeconds(10)));
            Assert.False(client1.Release());
        }
        Assert.True(client.Release());
    }

    var key = Guid.NewGuid().ToString("N");
    using (var client = RedisManager.GetRedLockClient(key))
    {
        Assert.True(client.TryLock(TimeSpan.FromSeconds(20)));
    }

    using (var client = RedisManager.GetRedLockClient(key))
    {
        Assert.True(client.TryLock(TimeSpan.FromMinutes(3)));
        Assert.True(client.Release());
    }
    ```

1. Rank 排行榜

    ``` csharp
    var rankClient = RedisManager.GetRankClient<string>("testRank");
    Assert.Equal(0, rankClient.Length());
    rankClient.Add("xiaoming", 100);
    rankClient.Add("xiaohong", 95);
    rankClient.Add("xiaowang", 96);
    Assert.Equal(3, rankClient.Length());
    Assert.Equal(100, rankClient.Score("xiaoming"));
    var rank = rankClient.RangeByScore();
    Assert.Equal("xiaohong", rank[0]);
    rank = rankClient.RangeByScore(order: Order.Descending);
    Assert.Equal("xiaoming", rank[0]);
    var common = RedisManager.GetCommonRedisClient(RedisDataType.Rank);
    Assert.True(common.KeyDelete("testRank"));
    ```

1. 更多用法等你来发现...

## Conatct

Contact me: <weihanli@outlook.com>