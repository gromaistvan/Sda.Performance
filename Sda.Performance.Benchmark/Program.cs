using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using static BenchmarkDotNet.Running.BenchmarkRunner;

namespace Sda.Performance.Benchmark
{
    [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 20)]
    public class Program
    {
        static readonly Uri _host = new Uri("http://localhost:5000");

        static readonly HttpClient _client = new HttpClient();

        static async Task CallAsync(string command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var url = new Uri(_host, $"api/{command}");
            await Task.WhenAll(Enumerable.Range(1, 3).Select(i => _client.GetAsync(url)));
        }

        [Benchmark(Description = "Synchronized")]
        public async Task SynchronizedTestAsync() => await CallAsync("sync");

        [Benchmark(Description = "Asynchronous")]
        public async Task AsynchronousTestAsync() => await CallAsync("async");

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
