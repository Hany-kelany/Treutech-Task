using System.ComponentModel.DataAnnotations;

namespace Task_TrueTech.Models;
public class StockItem
{
    [Key]
    public int StockItemId { get; set; }
    public int StockId { get; set; }
    public Stock stock { get; set; }
    public int ItemId { get; set; }
    public Item item { get; set; }
    public int Quantity { get; set; }
}