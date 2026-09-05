using SmallTable.Bloom;
using SmallTable.Util;

public interface ISmallTableEngine
{
    void Put(Key key, Entry entry);
    void Get(string key);
    void Stop();
}

public sealed class SmallTableSmallTableEngine : ISmallTableEngine
{
    private readonly IWalStorage _walstorage;
    private readonly IMemTableService _memTableService;
    private readonly IBloomService _bloomService;


    public SmallTableSmallTableEngine(IWalStorage walstorage,
        IMemTableService memTableService,IBloomService  bloomService)
    {
        _walstorage = walstorage;
        _memTableService = memTableService;
        _bloomService = bloomService;
    }

    public void Put(Key key, Entry entry)
    {
        _walstorage.Put(key, entry);
        _memTableService.Put(key.key.StringAsMemory(), entry.Value);
    }

    public ReadOnlySpan<byte> Get(ReadOnlySpan<byte> key)
    {
        if (!_bloomService.Exist(key))
            return null;

        return _memTableService.Get(key);
    }

    public void Stop()
    {
        _memTableService.Stop();
    }
}