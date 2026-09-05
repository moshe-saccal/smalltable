using SmallTable.MemTable;

namespace SmallTable.Engine.Test;

public class UnitTest1
{
    [Fact]
    public void TestCrc()
    {
       var a = Crc32.Crc( "ass"u8);
       Assert.Equal(0x7B4A72B6u, a);
       
    }

    [Fact]
    public void Test2()
    {
        // var mem = new MemTableService();
        // mem.Put("hola","como estas?");
        // mem.Put("hola2","como andas2?");
        //
    }
}
