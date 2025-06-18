using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Task_TrueTech.Models;

public class Item
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int TotalQuantity { get; set; }
    [ValidateNever]
    public ICollection<StockItem> StockItems { get; set; }
}