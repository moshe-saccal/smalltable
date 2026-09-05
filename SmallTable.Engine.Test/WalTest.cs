using Microsoft.Extensions.DependencyInjection;
using SmallTable.Util;

namespace SmallTable.Engine.Test;

public class WalTest : BaseTestDI
{
    [Fact]
    public void Test()
    {
        var host = this.GetHost();
        using var scope = host.Services.CreateScope();
        var walStorage = scope.ServiceProvider.GetRequiredService<IWalStorage>();
        
        walStorage.Put(new Key("test", "name", DateTime.Now.Ticks)
            , new Entry("".StringAsMemory(), false));
        
    }
}