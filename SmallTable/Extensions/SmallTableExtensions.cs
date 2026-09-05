using Microsoft.Extensions.DependencyInjection;
using SmallTable.Bloom;

using SmallTable.MemTable;
using SmallTable.Persistor;
using SmallTable.Storage.SSTable;

namespace SmallTable.Extensions;

public static class SmallTableExtensions
{
    public static void AddSmallTables(this IServiceCollection services)
    {
        services.Configure<WalStorageOptions>(c =>
        {
            c.BasePath = "C:\\dev\\temp\\";
        });

        services.AddScoped<IBloomService, BloomService>();
        services.AddScoped<ISSTableStorageMediaProvider, SSTableStorageMediaProviderInDisk>();
        services.AddScoped<ISSTableStorage, SSTableDisk>();
        services.AddScoped<ISmallTableEngine, SmallTableSmallTableEngine>();
        services.AddScoped<IWalStorage, WalStorageJson>();
        services.AddScoped<InRamMemoryStreamProvider, InRamMemoryStreamProvider>();
        services.AddScoped<IMemTableService, MemTableService>();
        services.AddScoped<IMemTableStorageProvider, MemTableStorageProvider>();
        services.Configure<MemTableOptions>(c =>
        {
            c.MemTableStorageProviderType=typeof(MemTableStorageAsDictionary);
        });

        services.AddScoped<IPersistorBroker,PersistorBroker>();
        
    }
}