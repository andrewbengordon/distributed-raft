namespace DistributedRaft.Data;

public class RequestVoteRequest
{
    public int Term { get; set; }
    public string CandidateId { get; set; }
}