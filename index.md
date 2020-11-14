# WeihanLi.Redis[![WeihanLi.Redis](https://img.shields.io/nuget/v/WeihanLi.Redis.svg)](https://www.nuget.org/packages/WeihanLi.Redis/)

[中文](./README.md)

## Build

[![Build Status](https://weihanli.visualstudio.com/Pipelines/_apis/build/status/WeihanLi.WeihanLi.Redis?branchName=dev)](https://weihanli.visualstudio.com/Pipelines/_build/latest?definitionId=15&branchName=dev)

[![Build Status](https://travis-ci.org/WeihanLi/WeihanLi.Redis.svg?branch=dev)](https://travis-ci.org/WeihanLi/WeihanLi.Redis)

## Intro

RedisExtensions for StackExchange.Redis, much more easier for generic opeartions,and supply some extensions for your business logics.

extensions based on redis 5 basic types:

1. String

    - Cache
    - Counter
    - Firewall
    - RedLock
    - RateLimiter

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

### Install

``` bash
dotnet add package WeihanLi.Redis
```

### Configuration

  1. Configue

  ``` csharp
  RedisManager.AddRedisConfig(config =>
      {
          config.CachePrefix = "WeihanLi.Redis.UnitTest";
          config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
      });
  ```

  or

  ``` csharp
  serviceCollection.AddRedisConfig(config =>
      {
          config.CachePrefix = "WeihanLi.Redis.UnitTest";
          config.ChannelPrefix = "WeihanLi.Redis.UnitTest";
          config.EnableCompress = false;// disable compress
      });
  ```

### Basic usage

1. Cache

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

1. Counter

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

1. Firewall

    ``` csharp
    var firewallName = "firewallTest";
    var firewallClient = RedisManager.GetFirewallClient(firewallName, TimeSpan.FromSeconds(3));
    Assert.True(firewallClient.Hit());
    Assert.False(firewallClient.Hit());
    await Task.Delay(TimeSpan.FromSeconds(3));
    Assert.True(firewallClient.Hit());
    ```

1. RedLock

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

1. Rank

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

1. DependencyResolver

    ``` csharp
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

1. More fancy usage, wait for your explore...

