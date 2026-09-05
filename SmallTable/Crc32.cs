public static class Crc32
{
    public static uint Crc(ReadOnlySpan<byte> data)
    {
        uint crc = 0xFFFFFFFF;
        const uint polynomial = 0xEDB88320;
        foreach (byte b in data)
        {
            crc ^= b; // xor
            
            for (int i = 0; i < 8; i++)
            {
                bool last = (crc & 1) != 0;
                crc = (crc >> 1); // move
                if (last)
                {
                    crc^= polynomial;
                }
            }
        }

        return ~crc;
    }
}