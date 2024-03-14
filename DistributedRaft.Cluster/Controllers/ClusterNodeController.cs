using DistributedRaft.Data;
using Microsoft.AspNetCore.Mvc;

namespace DistributedRaft.Cluster.Controllers;

[ApiController]
[Route("api/cluster-node")]
public class ClusterNodeController(HttpClient httpClient, ClusterNodeService clusterNodeService) : Controller
{
    private static readonly string NodeIdentifier = Environment.GetEnvironmentVariable("NODE_IDENTIFIER") ?? string.Empty;
    private static readonly string[] OtherNodeUrls = Environment.GetEnvironmentVariable("OTHER_NODE_URLS")?.Split(";") ?? Array.Empty<string>();
    
    [HttpPost("request-vote")]
    public async Task<IActionResult> RequestVote(RequestVoteRequest request)
    {
        if (request.Term > clusterNodeService.CurrentTerm)
        {
            clusterNodeService.CurrentTerm = request.Term;
            clusterNodeService.VotedFor = null;
        }
        
        if (request.Term < clusterNodeService.CurrentTerm)
        {
            return Ok(new
            {
                Term = clusterNodeService.CurrentTerm,
                VoteGranted = false
            });
        }
        
        if (clusterNodeService.VotedFor == null || clusterNodeService.VotedFor == request.CandidateId)
        {
            clusterNodeService.VotedFor = request.CandidateId;
            return Ok(new
            {
                Term = clusterNodeService.CurrentTerm,
                VoteGranted = true
            });
        }
        
        return Ok(new
        {
            Term = clusterNodeService.CurrentTerm,
            VoteGranted = false
        });
    }
    
    [HttpGet("strong-get")]
    public IActionResult StrongGet([FromQuery] string key)
    {
        if (clusterNodeService.Data.TryGetValue(key, out var value))
        {
            return Ok(value);
        }
        
        return NotFound();
    }
    
    [HttpPost("compare-and-swap")]
    public IActionResult CompareAndSwap(CompareAndSwapRequest request)
    {
        if (clusterNodeService.Data.TryGetValue(request.Key, out var value) && value.Value == request.OldValue)
        {
            clusterNodeService.Data[request.Key].Value = request.NewValue;
            return Ok();
        }
        
        return NotFound();
    }
    
    [HttpPost("append-entries")]
    public async Task<IActionResult> AppendEntries(AppendEntriesRequest request)
    {
        if (request.Term > clusterNodeService.CurrentTerm)
        {
            clusterNodeService.CurrentTerm = request.Term;
            clusterNodeService.VotedFor = null;
        }
        
        return Ok();
    }
    
    [HttpGet("eventual-get")]
    public IActionResult EventualGet([FromQuery] string key)
    {
        if (clusterNodeService.Data.TryGetValue(key, out var value))
        {
            return Ok(value);
        }
        
        return NotFound();
    }
}