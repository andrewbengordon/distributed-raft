namespace DistributedRaft.Data;

public class AppendDataRequest
{
    public string Key { get; set; }
    public string Value { get; set; }
}