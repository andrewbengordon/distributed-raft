namespace DistributedRaft.Data;

public class AppendEntriesRequest
{
    public int Term { get; set; }
    public string LeaderId { get; set; }
}