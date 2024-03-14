using Microsoft.AspNetCore.Mvc;

namespace DistributedRaft.Gateway.Controllers;

[ApiController]
[Route("api/gateway-node")]
public class GatewayNodeController(HttpClient httpClient, ILogger<GatewayNodeController> logger) : Controller
{
    private static readonly List<string> ClusterNodeUrls =
        Environment.GetEnvironmentVariable("CLUSTER_NODES")?.Split(";").ToList() ?? [];

    [HttpGet("strong-get")]
    public async Task<IActionResult> StrongGet([FromQuery] string key)
    {
        logger.LogInformation("StrongGet: {Key}", key);
        var result = await httpClient.GetStringAsync($"{ClusterNodeUrls[0]}/api/cluster-node/strong-get?key={key}");
        return Ok(result);
    }

    [HttpGet("eventual-get")]
    public async Task<IActionResult> EventualGet([FromQuery] string key)
    {
        logger.LogInformation("EventualGet: {Key}", key);
        var result = await httpClient.GetStringAsync($"{ClusterNodeUrls[0]}/api/cluster-node/eventual-get?key={key}");
        return Ok(result);
    }
    
    [HttpPost("compare-and-swap")]
    public async Task<IActionResult> CompareAndSwap(CompareAndSwapRequest request)
    {
        logger.LogInformation("CompareAndSwap: Key: {Key}, OldValue: {OldValue}, NewValue: {NewValue}", request.Key, request.OldValue, request.NewValue);
        var result = await httpClient.PostAsJsonAsync($"{ClusterNodeUrls[0]}/api/cluster-node/compare-and-swap", request);
        return Ok(result);
    }
}

public class CompareAndSwapRequest
{
    public string Key { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}
