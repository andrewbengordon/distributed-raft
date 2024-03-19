using DistributedRaft.Shop.Models;

namespace DistributedRaft.Shop.Services;

public class KeyValueStoreService(HttpClient httpClient)
{
    public async Task<KeyValueItem?> GetItemAsync(string key)
    {
        var response = await httpClient.GetFromJsonAsync<KeyValueItem>($"api/gateway-node/strong-get/{key}");
        return response;
    }
    
    public async Task AddItemAsync(KeyValueItem item)
    {
        await httpClient.PostAsJsonAsync("api/gateway-node/compare-and-swap", item);
    }
}