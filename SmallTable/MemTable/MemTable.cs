namespace SmallTable.MemTable;

public class MemTable(IMemTableStreamProvider streamProvider) 
{
    private readonly Stream _stream = streamProvider.GetStream();
    
    public void Write(ReadOnlyMemory<byte> key, ReadOnlyMemory<byte> value)
    {
        
    }
}

