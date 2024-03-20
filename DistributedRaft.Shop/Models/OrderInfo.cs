namespace DistributedRaft.Shop.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class OrderInfo
{
  [JsonPropertyName("orderId")]
  public string? OrderId { get; set; }

  [JsonPropertyName("purchaserUsername")]
  public string PurchaserUsername { get; set; }

  [JsonPropertyName("products")]
  public List<string> Products { get; set; }
}
