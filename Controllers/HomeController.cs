namespace Task_TrueTech.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public IActionResult Index()
    {
        var products = context.items
    .Select(item => new ItemVM
    {
       ItemId= item.ItemId,
       ItemName= item.ItemName,
        Description = item.Description,
        price=item.Price,
        TotalQuantity= item.TotalQuantity
        //TotalQuantity = item.StockItems.Sum(si => si.Quantity)
    }).ToList();

        return View(products);
    }


    [HttpGet()]
    public IActionResult BuyNow(int itemId )
    {
        var item = context.items.Find(itemId);
        if (item == null)
        {
            return NotFound();
        }
        ViewBag.name = item.ItemName;
        ViewBag.price = item.Price;
        ViewBag.ItemId = itemId;
        ViewBag.Cities = context.stocks.Select(s => s.City).Distinct().ToList();
        return View();
    }

    [HttpPost]
    public IActionResult BuyNow(BuyVM item)
    {
        ViewBag.Cities = context.stocks.Select(s => s.City).Distinct().ToList();

        if (item.Quantity <= 0)
        {
            TempData["ErrorMessage"] = "Required quantity must Greater Than zero.";
            return View("BuyNow", item);
        }

        var stock = context.stocks
            .Include(s => s.StockItems)
            .FirstOrDefault(s => s.City == item.City && s.StockItems.Any(si => si.ItemId == item.itemId));

        if (stock == null)
        {
            ModelState.AddModelError("", "الكمية المطلوبة غير متوفرة.");

            TempData["ErrorMessage"] = "this item not available in this stock now.";
            return View("BuyNow", item);
        }

        var stockItem = stock.StockItems.FirstOrDefault(si => si.ItemId == item.itemId);

        if (stockItem == null || stockItem.Quantity < item.Quantity)
        {
            TempData["ErrorMessage"] = "Required Quantity not available.";
            return View("BuyNow" , item);
        }

        stockItem.Quantity -= item.Quantity;
        var newitem = context.items.FirstOrDefault(i => i.ItemId == item.itemId);
        if(newitem!=null)
        {
            newitem.TotalQuantity -= item.Quantity;
        }


        context.SaveChanges();

        TempData["SuccessMessage"] = "Payment proccess done succesfully...";
        return RedirectToAction("Index");
    }







    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
