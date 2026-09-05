namespace SmallTable.Bloom;

public interface IBloomService
{
    public bool Exist(ReadOnlySpan<byte> key);
    public void Add(ReadOnlySpan<byte> key);
    public int[] Counters();
}