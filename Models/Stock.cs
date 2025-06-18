namespace Task_TrueTech.Models;

public class Stock
{
    public int StockId { get; set; }
    public string StockName { get; set; }
    public string City { get; set; }
    [ValidateNever]
    public ICollection<StockItem> StockItems { get; set; }
}
