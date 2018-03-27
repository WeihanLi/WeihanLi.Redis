using System.Linq;
using Autofac;
using StackExchange.Redis;
using WeihanLi.Common;
using WeihanLi.Common.Log;
using WeihanLi.Extensions;
using WeihanLi.Redis.Internals;

namespace WeihanLi.Redis
{
    public interface IRedisClient
    {
    }

    internal abstract class BaseRedisClient
    {
        public IRedisWrapper Wrapper { get; }

        /// <summary>
        /// logger
        /// </summary>
        protected ILogHelper Logger { get; }

        static BaseRedisClient()
        {
            var configurationOptions = new ConfigurationOptions
            {
                Password = RedisManager.RedisConfiguration.Password,
                DefaultDatabase = RedisManager.RedisConfiguration.DefaultDatabase,
                ConnectRetry = RedisManager.RedisConfiguration.ConnectRetry,
                ConnectTimeout = RedisManager.RedisConfiguration.ConnectTimeout,
                AllowAdmin = RedisManager.RedisConfiguration.AllowAdmin,
                Ssl = RedisManager.RedisConfiguration.Ssl,
                Proxy = RedisManager.RedisConfiguration.Proxy,
                AbortOnConnectFail = RedisManager.RedisConfiguration.AbortOnConnectFail,
                SyncTimeout = RedisManager.RedisConfiguration.SyncTimeout
            };
            configurationOptions.EndPoints.AddRange(RedisManager.RedisConfiguration.RedisServers.Select(s => Helpers.ConvertToEndPoint(s.Host, s.Port)).ToArray());

            var container = new ContainerBuilder();
            container.RegisterInstance(ConnectionMultiplexer.Connect(configurationOptions));
            DependencyResolver.SetDependencyResolver(new AutofacDependencyResolver(container.Build()));
        }

        protected BaseRedisClient(ILogHelper logger, IRedisWrapper redisWrapper)
        {
            Logger = logger;
            Wrapper = redisWrapper;
            var connection = DependencyResolver.Current.GetService<ConnectionMultiplexer>();
            Wrapper.Database = connection.GetDatabase();
            Wrapper.Subscriber = connection.GetSubscriber();
        }
    }
}
