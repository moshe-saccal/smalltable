using System.Buffers.Binary;
using System.Collections;

namespace SmallTable.Bloom;

public class BloomService : IBloomService
{
    public const int SIZE = 10000;
    private BitArray _bitArray = new BitArray(SIZE);
#if DEBUG
    private int[] _counters = new int[SIZE];
#endif

    public bool Exist(ReadOnlySpan<byte> key)
    {
        Decode(key, out var values);
        return _bitArray[values.Item1] && _bitArray[values.Item2] && _bitArray[values.Item3];
    }

    public void Add(ReadOnlySpan<byte> key)
    {
        Decode(key, out var values);
        _bitArray[values.Item1] = true;
        _bitArray[values.Item2] = true;
        _bitArray[values.Item3] = true;
        _bitArray[values.Item4] = true;
#if DEBUG
        _counters[values.Item1] += 1;
        _counters[values.Item2] += 1;
        _counters[values.Item3] += 1;
        _counters[values.Item4] += 1;
#endif
    }

    public int[] Counters()
    {
#if DEBUG
        return _counters;
#endif
        return [];
    }

    private void Decode(ReadOnlySpan<byte> key, out (int, int, int, int) values)
    {
        Span<byte> res = stackalloc byte[32];

        if (!System.Security.Cryptography.SHA256.TryHashData(key, res, out _))
        {
            throw new ApplicationException("Error computing hash");
        }

        values.Item1 = (int)(BinaryPrimitives.ReadUInt64LittleEndian(res[..8]) % SIZE);
        values.Item2 = (int)(BinaryPrimitives.ReadUInt64LittleEndian(res[8..16]) % SIZE);
        values.Item3 = (int)(BinaryPrimitives.ReadUInt64LittleEndian(res[16..24]) % SIZE);
        values.Item4 = (int)(BinaryPrimitives.ReadUInt64LittleEndian(res[24..32]) % SIZE);
    }
}