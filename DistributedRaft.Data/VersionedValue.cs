namespace DistributedRaft.Data;

public class VersionedValue<T>
{
    public T Value { get; set; }
    public int Version { get; set; }
}