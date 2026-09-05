using System.Collections.Concurrent;
using SmallTable.Storage.SSTable;

namespace SmallTable.Persistor;

public class PersistorBroker : IPersistorBroker
{
    private readonly ISSTableStorage _ssTableStorage;

    public PersistorBroker(ISSTableStorage  ssTableStorage)
    {
        _ssTableStorage = ssTableStorage;
    }
    
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentQueue<IMemTableStorage> _memTables = new ConcurrentQueue<IMemTableStorage>();

    public void Persist( IMemTableStorage memTable)
    {
        Enqueue(memTable);
    }

    public void Stop()
    {
        cts.Cancel();   
    }

    private async void Enqueue(IMemTableStorage memTable)
    {
        _memTables.Enqueue(memTable);
        await Tick();
    }

    private CancellationTokenSource cts= new CancellationTokenSource();
    private async Task Tick()
    {
        
        await _semaphore.WaitAsync(cts.Token);
        while (_memTables.TryDequeue(out var memTable))
        {
            if (cts.Token.IsCancellationRequested)
            {
                return;
            }
            await _ssTableStorage.Write(memTable);
        }
    }

  
}

public interface IPersistorBroker
{
    void Persist(IMemTableStorage memTable);
    void Stop();
}