using System.Net;

namespace WeihanLi.Redis.Internals
{
    internal static class Helpers
    {
        public static EndPoint ConvertToEndPoint(string host, int port)
        {
            if (IPAddress.TryParse(host, out var address))
            {
                return new IPEndPoint(address, port);
            }
            return new DnsEndPoint(host, port);
        }
    }
}
