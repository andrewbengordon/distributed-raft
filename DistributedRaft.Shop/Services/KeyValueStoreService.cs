using System.Net.Http.Json;
using DistributedRaft.Shop.Models;

namespace DistributedRaft.Shop.Services;

public class KeyValueStoreService
{
    private readonly HttpClient _httpClient;
    
    public KeyValueStoreService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<KeyValueItem> GetItemAsync(string key)
    {
        var response = await _httpClient.GetFromJsonAsync<KeyValueItem>($"api/gateway-node/strong-get/{key}");
        if (response == null) throw new InvalidOperationException();
        return response;
    }
    
    public async Task AddItemAsync(KeyValueItem item)
    {
        await _httpClient.PostAsJsonAsync("KeyValue", item);
    }
}