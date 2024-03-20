namespace DistributedRaft.Shop.Models;

public class ProductStockItem
{
  public string Key { get; set; }
  public int Value { get; set; }
  public int AdjustmentAmount { get; set; }
}