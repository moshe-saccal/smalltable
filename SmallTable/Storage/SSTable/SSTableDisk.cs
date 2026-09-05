namespace SmallTable.Storage.SSTable;

public class SSTableStorageMediaProviderInMEMORY : ISSTableStorageMediaProvider
{
    private MemoryStream _memoryStream;

    public SSTableStorageMediaProviderInMEMORY()
    {
        _memoryStream = new MemoryStream();
    }

    public IBinaryWriter Provide()
    {
        return new SSTableDisk.BinaryWriter2(_memoryStream);
    }
}

public class SSTableStorageMediaProviderInDisk : ISSTableStorageMediaProvider
{
    private StreamWriter _stream;

    public SSTableStorageMediaProviderInDisk()
    {
        var sw = new System.IO.StreamWriter("C:\\dev\\st_writer\\file.dat");
        _stream = new(sw.BaseStream);
    }

    public IBinaryWriter Provide()
    {
        return new SSTableDisk.BinaryWriter2(_stream.BaseStream);
    }
}

public interface ISSTableStorageMediaProvider
{
    public IBinaryWriter Provide();
}

public interface ISSTableStorage
{
    Task Write(IMemTableStorage table);
}

public class SSTableDisk : ISSTableStorage
{
    IBinaryWriter _writer;

    public SSTableDisk(ISSTableStorageMediaProvider provider)
    {
        _writer = provider.Provide();
    }

    public async Task Write(IMemTableStorage table)
    {
        internalWrite(table, _writer);
    }

    private void internalWrite(
        IMemTableStorage table, IBinaryWriter writer)
    {
        //calculate footer.
        //write all blocks.
        // block inde
        //write footer. x

        int index = 0;

        Span<BlockIndexEntry> indexes = stackalloc BlockIndexEntry[table.Count];


        var totalSize = table.KeySizes + table.ValueSizes + sizeof(int) + sizeof(int) + 1;

        writer.WriteInt(table.KeySizes); // total size
        writer.WriteInt(table.Count); //4 bytes


        for (int i = 0; i < table.Count; i++)
        {
            var current = table.GetTuple(i);

            indexes[index] = new BlockIndexEntry((byte)current.Key.Length, (int)writer.Position,
                sizeof(int) * 2 + 1 + current.Key.Length + current.Value.Value.Length);

            writer.WriteInt(current.Key.Length); // length 4 bytes offset 4 
            writer.WriteMem(current.Key); // key ... (len)         offset 4+lenk
            writer.WriteInt(current.Value.Value.Length); // len 4 b offset 4+lenk+4
            writer.WriteMem(current.Value.Value); // (len)
            writer.WriteBool(current.Value.Deleted); //0/1
            index++;
        }

        var indexStart = writer.Position;
        var acum = 0;
        for (int ix = 0; ix < indexes.Length; ix++)
        {
            var ele = table.GetTuple(ix);
            writer.WriteInt(ele.Key.Length); //1
            writer.WriteMem(ele.Key); // 1+n
            writer.WriteInt(acum); // 1
            // key len + int(key) + value len + value + deleted
            acum += ele.Key.Length + sizeof(int) + ele.Value.Value.Length + sizeof(int) + 1;
            writer.WriteInt(acum); //1 
        }

        var indexEnd = writer.Position;
        writer.WriteLong(indexStart); // index startpos; 8 bytes [-24 .. -16]
        writer.WriteLong(indexEnd); // index ends ; 8 bytes [-16.. -8]
        writer.WriteInt(1); //version 4 bytes
        writer.WriteInt(7437); // magic 4 bytes
        writer.Flush();
    }

    struct BlockIndexEntry(byte keyLen, int start, int length)
    {
        public byte KeyLen => keyLen;
        public int Start => start;
        public int Length => length;
    }

    public sealed class StorageBlock
    {
        public int Length { get; set; }
        public required byte[] Data { get; set; }
        public string Crc32 { get; set; } = string.Empty;
    }

    public sealed class StorageBlockIndexEntry
    {
        public string Key { get; set; } = string.Empty;
        public int Offset { get; set; } = 0;
    }


    public sealed class BinaryWriter2(Stream stream) : IBinaryWriter
    {
        private readonly BinaryWriter _writer = new BinaryWriter(stream);
        public int Length { get; private set; }

        public void WriteBool(bool b)
        {
            Length += 1;

            _writer.Write(b);
            Log();
        }

        public long Position => Length;

        private void Log()
        {
            Console.WriteLine("Len is :" + Length);
        }

        public void WriteMem(ReadOnlyMemory<char> buffer)
        {
            Length += buffer.Length;
            _writer.Write(buffer.ToArray());
            Log();
        }

        public void WriteMem(ReadOnlyMemory<byte> buffer)
        {
            Length += buffer.Length;
            _writer.Write(buffer.ToArray());
            Log();
        }

        public void WriteLong(long value)
        {
            Length += sizeof(long);
            _writer.Write(value);
            Log();
        }

        public void WriteLongAsInt(long value)
        {
            WriteInt((int)value);
        }

        public void WriteInt(int value)
        {
            Length += sizeof(int);
            _writer.Write(value);
            Log();
        }

        public void Flush()
        {
            _writer.Flush();
            if (stream is FileStream fs)
                fs.Flush(true);
        }
    };
}

public interface IBinaryWriter
{
    public void Flush();
    public void WriteInt(int value);
    public void WriteLong(long value);
    public void WriteMem(ReadOnlyMemory<byte> buffer);
    public void WriteBool(bool b);
    public long Position { get; }
}