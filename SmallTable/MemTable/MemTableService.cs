// using System.Text.Unicode;
// using Microsoft.Extensions.Options;
//
// namespace SmallTable.MemTable;
//

using System.Collections;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using SmallTable.MemTable;
using SmallTable.Persistor;

public class MemTableService : IMemTableService
{
    private readonly IMemTableStorageProvider _provider;
    private readonly IPersistorBroker _persistorBroker;
    private readonly MemTableOptions _options;
    private readonly List<IMemTableStorage> _tables = new();
    private IMemTableStorage _active;

    public MemTableService(IOptions<MemTableOptions> options,
        IMemTableStorageProvider provider, IPersistorBroker persistorBroker)
    {
        _provider = provider;
        _persistorBroker = persistorBroker;
        _options = options.Value;
        _active = _provider.Provide();
        _tables.Add(_active);
    }

    public const int MAX_SIZE = 10;


    public Entry Get(string key)
    {
        return null;
    }

    public void Delete(string key)
    {
    }

    public List<string> GetKeys()
    {
        return [];
    }

    private void internalPut(string key, ReadOnlySpan<char> value)
    {
    }

    public void Put(ReadOnlyMemory<byte> key, ReadOnlyMemory<byte> value)
    {
        _active.Add(key, value);
        if (_active.isFull())
        {
            var current = _active;
            _active = _provider.Provide();
            _tables.Add(_active);
            _persistorBroker.Persist(current);
        }
    }

    public void Stop()
    {
        _persistorBroker.Stop();
    }

    public byte Contains(ReadOnlySpan<byte> key)
    {
        if (_active.Contains(key))
            return byte.MinValue;

        foreach (var t in _tables)
        {
            if (t.Contains(key))
                return (byte)_tables.IndexOf(t);
        }

        return byte.MaxValue;
    }

    public ReadOnlySpan<byte> Get(ReadOnlySpan<byte> key)
    {
        var where = this.Contains(key);
        if (where == byte.MaxValue)
            return [];
        
        if(where ==byte.MinValue)
            return _active[]
            
    }
}

//
public class MemTableStorageProvider : IMemTableStorageProvider
{
    private Type? type = null;
    private IMemTableStorage? _provider = null;

    public MemTableStorageProvider(IOptions<MemTableOptions> options)
    {
        type = options.Value.MemTableStorageProviderType;
    }

    public IMemTableStorage Provide()
    {
        _provider = (IMemTableStorage)Activator.CreateInstance(type)!;

        return _provider;
    }
}

public interface IMemTableStorageProvider
{
    public IMemTableStorage Provide();
}

public struct ValueValue
{
    public ValueValue(ReadOnlyMemory<byte> value, bool deleted)
    {
        Deleted = deleted;
        Value = value;
    }

    public ReadOnlyMemory<byte> Value { get; init; }
    public bool Deleted { get; init; }
}

public interface IMemTableStorage
{
    public bool Contains(ReadOnlySpan<byte> key);

    public IEnumerable<(ReadOnlyMemory<byte> Key, ReadOnlyMemory<byte> Value)> Entries { get; }
    void Add(ReadOnlyMemory<byte> key, ReadOnlyMemory<byte> value);
    public (ReadOnlyMemory<byte> Key, ValueValue Value) GetTuple(int ix);

    bool isFull();
    bool isStored();
    public int Count { get; }
    public int KeySizes { get; }
    public int ValueSizes { get; }
    public byte[] this[int index] { get; }

    public int TotalSize
    {
        get
        {
            var totalSize = KeySizes + ValueSizes + sizeof(int) + sizeof(int) + 1;
            return totalSize;
        }
    }
}

public class MemTableStorageAsDictionary : IMemTableStorage
{
    private List<(ReadOnlyMemory<byte> key, ValueValue value)> _memTable = new();
    private HashSet<ReadOnlyMemory<byte>> indexes = [];


    private int _size;


    public Int16 Contains(ReadOnlySpan<byte> key)
    {
        //todo: improve
        var arr = key.ToArray();
        var i = indexes.Contains(arr);
        
        
        
        return indexes.Contains(key.ToArray());
    }

    public IEnumerable<(ReadOnlyMemory<byte> Key, ReadOnlyMemory<byte> Value)> Entries { get; }

    public void Add(ReadOnlyMemory<byte> key, ReadOnlyMemory<byte> value)
    {
        indexes.Add(key);
        _memTable.Add((key.ToArray(), new ValueValue(value, false)));
        _size += key.Length;
        _size += value.Length;
    }

    public (ReadOnlyMemory<byte> Key, ValueValue Value) GetTuple(int ix)
    {
        return _memTable[ix];
    }


    public const int MAX_SIZE = 10;

    public bool isFull()
    {
        if (_memTable.Count == MAX_SIZE)
        {
            //Debugger.Break();
        }

        return _memTable.Count == MAX_SIZE;
    }

    private bool _isStored { get; set; } = false;
    public bool isStored() => _isStored;
    public int Count => _memTable.Count;

    public byte[] this[int index] => throw new NotImplementedException();

    public int TotalSize { get; set; }
    public int KeySizes => _memTable.Sum(x => x.Item1.Length);
    public int ValueSizes => _memTable.Sum(x => x.Item2.Value.Length);


    public IEnumerator<int> GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

public class ByteComparerByValue : IEqualityComparer<byte[]>
{
    public bool Equals(byte[]? x, byte[]? y)
    {
        if ((x?.Length ?? 0) != (y?.Length ?? 0))
            return false;

        for (int i = 0; i < x!.Length; i++)
        {
            if (x[i] != y![i])
                return false;
        }

        return true;
    }

    public int GetHashCode(byte[] obj)
    {
        return System.Text.Encoding.UTF8.GetString(obj).GetHashCode();
    }
}