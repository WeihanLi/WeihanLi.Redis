using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Redis.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddRedisConfig(options => { options.EnableCompress = true; });
            // custom serializer
            services.AddSingleton<IDataSerializer, BinaryDataSerializer>();
            services.AddSingleton<IDataCompressor, MockDataCompressor>();
            DependencyResolver.SetDependencyResolver(services);

            var cacheClient = DependencyResolver.Current.ResolveService<ICacheClient>();
            var customSerializerCacheKey = "TestCustomSerializer";
            cacheClient.Set(customSerializerCacheKey, new PagedListModel<int>()
            {
                Data = new int[0],
                PageNumber = 2,
                PageSize = 10,
                TotalCount = 10,
            }, TimeSpan.FromMinutes(1));
            var result = cacheClient.Get<PagedListModel<int>>(customSerializerCacheKey);
            Console.WriteLine(result.ToJson());

            Console.ReadLine();
        }

        private class MockDataCompressor : IDataCompressor
        {
            public byte[] Compress(byte[] sourceData)
            {
                return sourceData;
            }

            public Task<byte[]> CompressAsync(byte[] sourceData)
            {
                return Task.FromResult(sourceData);
            }

            public byte[] Decompress(byte[] compressedData)
            {
                return compressedData;
            }

            public Task<byte[]> DecompressAsync(byte[] compressedData)
            {
                return Task.FromResult(compressedData);
            }
        }
    }
}
