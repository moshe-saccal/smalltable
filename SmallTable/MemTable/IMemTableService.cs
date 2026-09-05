public interface IMemTableService
{
    void Put(ReadOnlyMemory<byte> key, ReadOnlyMemory<byte> value);
    void Stop();
    Int16 Contains(ReadOnlySpan<byte>key );
    ReadOnlySpan<byte> Get(ReadOnlySpan<byte>key );
}