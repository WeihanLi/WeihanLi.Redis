using StackExchange.Redis;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;

namespace WeihanLi.Redis.Samples
{
    public class ConfigurationChangedEventSample
    {
        private static readonly ILogHelperLogger Logger = LogHelper.GetLogger<ConfigurationChangedEventSample>();

        public static void MainTest()
        {
            var connectionMultiplexer = DependencyResolver.Current.ResolveService<IConnectionMultiplexer>();
            connectionMultiplexer.ConfigurationChanged += (sender, eventArgs) =>
            {
                Logger.Info($"ConfigurationChanged sender: {sender}, eventArgs: {eventArgs.EndPoint}");
            };
        }
    }
}
