using System.Buffers.Binary;
using Microsoft.Extensions.DependencyInjection;
using SmallTable.Bloom;
using Xunit.Abstractions;

namespace SmallTable.Engine.Test;

public class Bloom_Test : BaseTestDI
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Bloom_Test(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test_Bloom()
    {
        var host = this.GetHost();
        using var scope = host.Services.CreateScope();
        var bloom = scope.ServiceProvider.GetRequiredService<IBloomService>();

        var toGen = 1000000;
        var lis = new List<string>();

        for (int i = 0; i < toGen; i++)
        {
            lis.Add(RandomString(20, i % 3 == 0 ? 50 : 80));
            bloom.Add(System.Text.Encoding.UTF8.GetBytes(lis.Last()));
        }


        var res = bloom.Exist("hello"u8);
        res.ToString();
        var counters = bloom.Counters();
        var avg = counters.Average();
        
        var variance = counters
            .Select(x => Math.Pow(x - avg, 2))
            .Average();

        var stdDev = Math.Sqrt(variance);

        _testOutputHelper.WriteLine($"Average: {avg}");
        _testOutputHelper.WriteLine($"Variance: {variance}");
        _testOutputHelper.WriteLine($"StdDev: {stdDev}");
        
        for (int i = 0; i < counters.Length; i++)
        {
            var dist = Math.Abs(avg - counters[i]) * Math.Abs(avg - counters[i]);
            
            _testOutputHelper.WriteLine($"{i}  : {counters[i]} - {dist}");
        }
    }

    static string RandomString(int minLength = 1, int maxLength = 100)
    {
        const string chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        var length = Random.Shared.Next(minLength, maxLength + 1);

        return string.Create(length, chars, (span, source) =>
        {
            for (var i = 0; i < span.Length; i++)
                span[i] = source[Random.Shared.Next(source.Length)];
        });
    }
}