public class InRamMemoryStreamProvider:IMemTableStreamProvider
{
    private readonly MemoryStream _stream = new();

    public Stream GetStream()
    {
        return _stream;
    }
}