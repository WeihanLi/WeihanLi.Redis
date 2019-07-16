# WeihanLi.Redis[![WeihanLi.Redis](https://img.shields.io/nuget/v/WeihanLi.Redis.svg)](https://www.nuget.org/packages/WeihanLi.Redis/)

## Build

[![Build Status](https://travis-ci.org/WeihanLi/WeihanLi.Redis.svg?branch=dev)](https://travis-ci.org/WeihanLi/WeihanLi.Redis)

## Intro

RedisExtensions for StackExchange.Redis, much more easier for generic opeartions,and supply some extensions for your business logics.

StackExchange.Redis 扩展，更简单的泛型操作，并提供一些的适用于业务场景中的扩展

基于 Redis 的五种主要的数据类型做了一些扩展：

1. String

    - Cache
    - Counter
    - Firewall
    - RedLock

1. Hash

    - Hash
    - Dictonary
    - HashCounter

1. List

    - List
    - Queue
    - Stack

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

对于 V1.0.6 及以下版本支持 .netframework4.5 配置参考：

  1. 日志配置，日志基于 log4net 的配置，可以参考单元测试中的 `log4net.config` 的配置，将 `log4net.config` 放在项目根目录下【推荐】
  如果不在项目根目录下或者文件名发生修改则需要自己手动调用 `LogHelper.LogInit("log4net.config");`
  
  2. Redis 配置

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

对于 1.1* 及以上版本不再支持 dotnet framework，配置方式如下：

  ``` csharp
  // IServiceCollection serviceCollection = new ServiceCollection();

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

1. 依赖注入

    ``` csharp
    // 在 asp.net core 中可以直接从构造器注入，这里使用的一个 ServiceLocator 模式获取注入的服务
    var cacheClient = DependencyResolver.Current.ResolveService<ICacheClient>();
    Assert.NotNull(cacheClient);
    var key = Guid.NewGuid().ToString("N");
    Assert.Equal("abcaaa", cacheClient.GetOrSet(key, () => "abcaaa", TimeSpan.FromMinutes(10)));
    cacheClient.Remove(key);
    var hashClient = DependencyResolver.Current.ResolveService<IHashClient>();
    Assert.NotNull(hashClient);
    var pubsubClient = DependencyResolver.Current.ResolveService<IPubSubClient>();
    Assert.NotNull(pubsubClient);
    ```

1. 更多用法等你来发现...