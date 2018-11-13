using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using static BenchmarkDotNet.Running.BenchmarkRunner;

namespace Sda.Performance.Benchmark
{
    [SimpleJob(launchCount: 1, warmupCount: 4, targetCount: 20)]
    public class Program
    {
        static readonly Uri _host = new Uri("http://localhost:5000");

        static readonly HttpClient _client = new HttpClient();

        static async Task CallAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            var url = new Uri(_host, path);
            await Task.WhenAll(Enumerable.Range(1, 20).Select(i => _client.GetAsync(url)));
        }

        [Benchmark]
        public void Test() => CallAsync("api/entry/sync").Wait();

        [Benchmark]
        public async Task TestAsync() => await CallAsync("api/entry/async");

        static void Main()
        {
            try
            {
                Run<Program>();
            }
            catch (Exception)
            {
                // Ignore it!
            }
        }
    }
}
