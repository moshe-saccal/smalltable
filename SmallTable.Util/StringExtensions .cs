namespace SmallTable.Util;

public static class StringExtensions 
{
    public static ReadOnlyMemory<byte> StringAsMemory(this ReadOnlySpan<char> str)
    {
        return System.Text.Encoding.UTF8.GetBytes( str.ToArray());
    }
}