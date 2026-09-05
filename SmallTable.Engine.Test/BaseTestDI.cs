using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using SmallTable.Extensions;
using SmallTable.MemTable;

namespace SmallTable.Engine.Test;

public class BaseTestDI
{
    private IHost? _host = null;

    protected IHost GetHost()
    {
        if (_host is not null)
            return _host;

        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddSmallTables();

        _host = builder.Build();

        return _host;
    }

    public IMemTableStorageProvider GetMemStorageProvider()
    {
        return _host!.Services.GetRequiredService<IMemTableStorageProvider>();
        
    }
    public IMemTableService GetMemStorage()
    {
        return _host!.Services.GetRequiredService<IMemTableService>();
    }
    public IWalStorage GetWalStorage()
    {
        return _host!.Services.GetRequiredService<IWalStorage>();

    }
}