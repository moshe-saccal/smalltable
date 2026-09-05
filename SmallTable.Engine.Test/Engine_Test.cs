using Microsoft.Extensions.DependencyInjection;
using SmallTable.Util;
using Xunit.Abstractions;

namespace SmallTable.Engine.Test;

public class Engine_Test : BaseTestDI
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Engine_Test(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test_Write_Engine()
    {
        var host = this.GetHost();
        using var scope = host.Services.CreateScope();
        var engine = scope.ServiceProvider.GetRequiredService<ISmallTableEngine>();

        for (int i = 0; i < 100; i++)
        {
            _testOutputHelper.WriteLine(i.ToString());
            engine.Put(new Key($"test{i}", "name", DateTime.Now.Ticks)
                , new Entry($"value__{i}".StringAsMemory(), false));
        }
        engine.Stop();
        "".ToString();
    }

    [Fact]
    public void Test_Read_Engine()
    {
        var host = this.GetHost();
        using var scope = host.Services.CreateScope();
        var engine = scope.ServiceProvider.GetRequiredService<ISmallTableEngine>();
        engine.Get("test4");
    }
}