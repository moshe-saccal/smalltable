using Microsoft.Extensions.Options;

public class Wal<T>(T walStorage)
    where T : IWalStorage
{
    private readonly T _walStorage = walStorage;

    public void Put(Key key, Entry entry)
    {
        _walStorage.Put(key, entry);
    }

    public void Flush()
    {
    }

    public void Start()
    {
        //replay
        //rebuild mem table
    }
}

public interface IWalStorage
{
    void Put(Key key, Entry entry);
}

public class WalStorageMemory : IWalStorage
{
    private Dictionary<Key, Entry> _dictionary = new();

    public WalStorageMemory()
    {
    }

    public void Put(Key key, Entry entry)
    {
        _dictionary.Add(key, entry);
    }
}

public class WalStorageOptions
{
    public string BasePath { get; set; } = string.Empty;
}

public  class WalStorageJson(IOptions<WalStorageOptions> walStorageOptions) : IWalStorage
{
    public void Put(Key key, Entry entry)
    {
        File.WriteAllText(Path.Join(walStorageOptions.Value.BasePath, $"{key.key}-{key.timestamp}.json"),
            System.Text.Json.JsonSerializer.Serialize(entry));
    }
}