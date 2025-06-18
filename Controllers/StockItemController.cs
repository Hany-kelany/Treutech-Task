using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_TrueTech.Data;
using Task_TrueTech.Models;
using Task_TrueTech.ViewModel;

namespace Task_TrueTech.Controllers
{
    public class StockItemController : Controller
    {

        private readonly ApplicationDbContext _context;

        public StockItemController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()  // كل المخازن بكل المنتجات
        {
           return View();   
        }
        // GET: Add Item to Stock
        [HttpGet]
        public async Task<IActionResult> AddItemToStock(string city = null)
        {
            var stocks = _context.stocks.AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                stocks = stocks.Where(s => s.City == city);
            }

            ViewBag.Items = await _context.items.ToListAsync();
            ViewBag.Cities = await _context.stocks
                .Select(s => s.City)
                .Distinct()
                .ToListAsync();
            ViewBag.Stocks = await stocks.ToListAsync();

            return View(new AddingItemstoStockVM { City = city });
        }

        // POST: Add Item to Stock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemToStock(AddingItemstoStockVM model)
        {
            if (ModelState.IsValid)
            {
                // Check if the item already exists in the selected stock
                var existingBranchItem = await _context.stockItems
                    .FirstOrDefaultAsync(bi => bi.StockId == model.stockid && bi.ItemId == model.itemId);

                if (existingBranchItem != null)
                {
                    // Increase the quantity if the item exists
                    existingBranchItem.Quantity += model.quantity;
                    _context.stockItems.Update(existingBranchItem);
                }
                else
                {
                    // Add a new item if it doesn't exist in the stock
                    var branchItem = new StockItem
                    {
                        StockId = model.stockid,
                        ItemId = model.itemId,
                        Quantity = model.quantity
                    };
                    _context.stockItems.Add(branchItem);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Stock"); // Redirect to the stock list or details
            }

            // Reload dropdown data if there's an error
            ViewBag.Items = await _context.items.ToListAsync();
            ViewBag.Cities = await _context.stocks
                .Select(s => s.City)
                .Distinct()
                .ToListAsync();
            ViewBag.Stocks = await _context.stocks.ToListAsync();
            return View(model);
        }
    }
}
