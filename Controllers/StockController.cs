using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_TrueTech.Data;
using Task_TrueTech.Models;

namespace Task_TrueTech.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

            // List all stocks
            public async Task<IActionResult> Index()
            {
                var stocks = await _context.stocks
                    .Include(s => s.StockItems)
                    .ThenInclude(bi => bi.item)
                    .ToListAsync();
                return View(stocks);
            }

            // View details of a specific stock
            public async Task<IActionResult> Details(int id)
            {
                var stock = await _context.stocks
                    .Include(s => s.StockItems)
                    .ThenInclude(bi => bi.item)
                    .FirstOrDefaultAsync(s => s.StockId == id);

                if (stock == null)
                    return NotFound();

                return View(stock);
            }

            // Create a new stock
            [HttpGet]
            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Stock stock)
            {
                if (ModelState.IsValid)
                {
                    _context.stocks.Add(stock);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(stock);
            }

            // Edit an existing stock
            [HttpGet]
            public async Task<IActionResult> Edit(int id)
            {
                var stock = await _context.stocks.FindAsync(id);

                if (stock == null)
                    return NotFound();

                return View(stock);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, Stock stock)
            {
                if (id != stock.StockId)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.stocks.Update(stock);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!StockExists(stock.StockId))
                            return NotFound();
                        throw;
                    }

                    return RedirectToAction(nameof(Index));
                }
                return View(stock);
            }

            // Delete a stock
            [HttpGet]
            public async Task<IActionResult> Delete(int id)
            {
                var stock = await _context.stocks.FindAsync(id);

                if (stock == null)
                    return NotFound();

                return View(stock);
            }

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var stock = await _context.stocks.FindAsync(id);

                if (stock == null)
                    return NotFound();

                _context.stocks.Remove(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Add item to stock
            [HttpGet]
            public async Task<IActionResult> AddItem(int stockId)
            {
                var stock = await _context.stocks.FindAsync(stockId);

                if (stock == null)
                    return NotFound();

                ViewBag.Items = await _context.items.ToListAsync();
                return View(new StockItem { StockId = stockId });
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddItem(StockItem stochItem)
            {
                if (ModelState.IsValid)
                {
                    _context.stockItems.Add(stochItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = stochItem.StockItemId });
                }
                ViewBag.Items = await _context.items.ToListAsync();
                return View(stochItem);
            }

            // Helper method to check if stock exists
            private bool StockExists(int id)
            {
                return _context.stocks.Any(s => s.StockId == id);
            }
        }
    }


