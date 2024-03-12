using Microsoft.AspNetCore.Mvc;

namespace DistributedRaft.Gateway.Controllers;

[ApiController]
[Route("api/gateway-node")]
public class GatewayNodeController(HttpClient httpClient) : Controller
{
    private static readonly string[] ClusterNodeUrls = Environment.GetEnvironmentVariable("CLUSTER_NODES")?.Split(";") ?? Array.Empty<string>();

    [HttpGet("process")]
    public async Task<IActionResult> ProcessRequest()
    {
        Console.WriteLine("Processing request...");

        var response = await httpClient.GetAsync(ClusterNodeUrls[0] + "/status");
        
        Console.WriteLine($"Received response: {await response.Content.ReadAsStringAsync()}");

        return Ok("Request processed by the gateway.");
    }
}