using System.Buffers.Binary;
using System.Text;
using System.Text.Unicode;
using SmallTable.Extensions;
using SmallTable.MemTable;
using SmallTable.Util;

namespace SmallTable.Engine.Test;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class MemTableTest : BaseTestDI
{
    [Fact]
    public void Test_BasicStorage()
    {
        var host = this.GetHost();
        using var scope = host.Services.CreateScope();
        var memStorage = scope.ServiceProvider.GetRequiredService <IMemTableStorageProvider>();
        
        var memTableStorage = memStorage.Provide();

        memTableStorage.Add("test".StringAsMemory(), "test".StringAsMemory());
        memTableStorage.Add("hello".StringAsMemory(), "hello2".StringAsMemory());
        var content = memTableStorage.GetTuple(0);
        var contentStr =System.Text.Encoding.UTF8.GetString(  content.Value.Value.ToArray());
        contentStr.ToString();
    }

    private void VerifyStructure(Stream stream)
    {
        stream.Position = 0;
        byte[] buffer = new byte[stream.Length];

        stream.ReadAtLeast(buffer, (int)stream.Length);


        var magic = BinaryPrimitives.ReadInt32LittleEndian(buffer[^4..]);

        Assert.True(magic == 7437);

        var version = BinaryPrimitives.ReadInt32LittleEndian(buffer[^8..]);
        Assert.True(version == 1);

        var ixEnd = BinaryPrimitives.ReadInt64LittleEndian(buffer[^16..]);
        var ixSt = BinaryPrimitives.ReadInt64LittleEndian(buffer[^24..]);
        var ixSlice = buffer[(int)ixSt..(int)ixEnd];

        ixSlice.ToString();

        var total = ixEnd - ixSt;
        var start = (int)ixSt;
        int ix = start;
        while ((ix - start) < total)
        {
            var keySize = BinaryPrimitives.ReadInt32LittleEndian(buffer.AsSpan(ix));
            ix += sizeof(int);
            var key = System.Text.Encoding.UTF8.GetString(buffer.AsSpan(ix, keySize));
            ix += keySize;
            var initial = BinaryPrimitives.ReadInt32LittleEndian(buffer.AsSpan((int)(ix)));
            ix += sizeof(int);
            var finish = BinaryPrimitives.ReadInt32LittleEndian(buffer.AsSpan((int)(ix)));
            ix += sizeof(int);
            Console.WriteLine($"{key} , {initial} , {finish}");
            var content = buffer.AsSpan((initial + sizeof(int) + keySize + sizeof(int))..(finish - 1));
            Console.WriteLine($"Content:{System.Text.Encoding.UTF8.GetString(content)}");
        }
    }
}