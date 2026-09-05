public sealed record Key(string key, string column, long timestamp);
public sealed record Entry(ReadOnlyMemory<byte> Value,bool Deleted);