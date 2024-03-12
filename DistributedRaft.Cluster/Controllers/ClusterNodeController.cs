using Microsoft.AspNetCore.Mvc;

namespace DistributedRaft.Cluster.Controllers;

[ApiController]
[Route("api/cluster-node")]
public class ClusterNodeController(HttpClient httpClient) : Controller
{
    private static readonly string NodeIdentifier = Environment.GetEnvironmentVariable("NODE_IDENTIFIER") ?? string.Empty;
    private static readonly string[] OtherNodeUrls = Environment.GetEnvironmentVariable("OTHER_NODE_URLS")?.Split(";") ?? Array.Empty<string>();

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok($"Node {NodeIdentifier} is running.");
    }
    
    [HttpGet("status-of-other-nodes")]
    public async Task<IActionResult> GetStatusOfOtherNodes()
    {
        foreach (var url in OtherNodeUrls)
        {
            try
            {
                var response = await httpClient.GetAsync($"{url}/status");
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Failed to communicate with node at {url}.");
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Exception thrown when attempting to communicate with node at {url}: {e.Message}");
            }
        }

        return Ok($"Node {NodeIdentifier} successfully communicated with other nodes.");
    }
}