using DistributedRaft.Data;

namespace DistributedRaft.Cluster;

enum ClusterNodeState
{
    Follower,
    Candidate,
    Leader
}

public class ClusterNodeService(HttpClient httpClient, ILogger<ClusterNodeService> logger)
{
    private ClusterNodeState _state = ClusterNodeState.Follower;
    private readonly string _nodeIdentifier = Environment.GetEnvironmentVariable("NODE_IDENTIFIER") ?? string.Empty;
    private readonly string _nodeUrl = Environment.GetEnvironmentVariable("NODE_URL") ?? string.Empty;
    private readonly string[] _otherNodeUrls = Environment.GetEnvironmentVariable("OTHER_NODE_URLS")?.Split(";") ?? Array.Empty<string>();
    
    public int CurrentTerm { get; set; } = 0;
    public string VotedFor = null;
    public Dictionary<string, VersionedValue<string>> Data { get; set; } = new();    private readonly Dictionary<string, string> keyValueStore = new();
    private readonly Dictionary<string, int> keyVersionStore = new();
    private Timer electionTimer;
    
    public async Task StartElection()
    {
        if (_state == ClusterNodeState.Leader)
        {
            return;
        }
        
        _state = ClusterNodeState.Candidate;
        CurrentTerm++;
        VotedFor = _nodeIdentifier;
        var request = new
        {
            Term = CurrentTerm,
            CandidateId = _nodeIdentifier
        };
        var tasks = _otherNodeUrls.Select(url => httpClient.PostAsJsonAsync($"{url}/api/cluster-node/request-vote", request));
        var responses = await Task.WhenAll(tasks);
        var successfulVotes = responses.Count(response => response.IsSuccessStatusCode);
        if (successfulVotes >= _otherNodeUrls.Length / 2)
        {
            _state = ClusterNodeState.Leader;
            electionTimer.Change(Timeout.Infinite, Timeout.Infinite);
            await StartHeartbeat();
        }
    }
    
    public async Task StartHeartbeat()
    {
        while (_state == ClusterNodeState.Leader)
        {
            var request = new
            {
                Term = CurrentTerm,
                LeaderId = _nodeIdentifier
            };
            var tasks = _otherNodeUrls.Select(url => httpClient.PostAsJsonAsync($"{url}/api/cluster-node/append-entries", request));
            var responses = await Task.WhenAll(tasks);
            var successfulAppendEntries = responses.Count(response => response.IsSuccessStatusCode);
            if (successfulAppendEntries < _otherNodeUrls.Length / 2)
            {
                _state = ClusterNodeState.Follower;
                return;
            }
            await Task.Delay(100);
        }
    }
    
    public async Task RequestVote(RequestVoteRequest request)
    {
        if (request.Term > CurrentTerm)
        {
            _state = ClusterNodeState.Follower;
            CurrentTerm = request.Term;
            VotedFor = null;
        }
        if (request.Term < CurrentTerm)
        {
            return;
        }
        if (VotedFor == null || VotedFor == request.CandidateId)
        {
            VotedFor = request.CandidateId;
            var response = new
            {
                Term = CurrentTerm,
                VoteGranted = true
            };
            await httpClient.PostAsJsonAsync($"{request.CandidateId}/api/cluster-node/request-vote-response", response);
        }
    }
    
    public bool VoteForCandidate(RequestVoteRequest request)
    {
        if (request.Term < CurrentTerm)
        {
            return false;
        }
        if (VotedFor == null || VotedFor == request.CandidateId)
        {
            VotedFor = request.CandidateId;
            return true;
        }
        return false;
    }
    
    public async Task AppendEntries(AppendEntriesRequest request)
    {
        if (request.Term > CurrentTerm)
        {
            _state = ClusterNodeState.Follower;
            CurrentTerm = request.Term;
            VotedFor = null;
        }
    }
    
    public string StrongGet(string key)
    {
        if (Data.TryGetValue(key, out var value))
        {
            return value.Value;
        }
        return null;
    }
    
    public string EventualGet(string key)
    {
        if (Data.TryGetValue(key, out var value))
        {
            return value.Value;
        }
        return null;
    }
    
    public bool CompareAndSwap(CompareAndSwapRequest request)
    {
        if (Data.TryGetValue(request.Key, out var value))
        {
            if (value.Value == request.OldValue)
            {
                Data[request.Key] = new VersionedValue<string>
                {
                    Value = request.NewValue,
                    Version = value.Version + 1
                };
                return true;
            }
        }
        return false;
    }
    
    public void AppendData(AppendDataRequest request)
    {
        if (Data.TryGetValue(request.Key, out var value))
        {
            Data[request.Key] = new VersionedValue<string>
            {
                Value = request.Value,
                Version = value.Version + 1
            };
        }
        else
        {
            Data[request.Key] = new VersionedValue<string>
            {
                Value = request.Value,
                Version = 1
            };
        }
    }
}

