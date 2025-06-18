namespace Task_TrueTech.Controllers;
public class ItemController : Controller
{
    private readonly ApplicationDbContext context;

    public ItemController(ApplicationDbContext context)
    {
        this.context = context;
    }


    public IActionResult Index()
    {
        var items = context.items.Include(i => i.StockItems).ThenInclude(si => si.stock).ToList();
        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Items = context.items.ToList(); 
        ViewBag.Stocks = context.stocks.ToList(); 
        return View();
    }

    //[HttpPost]
    //public IActionResult Create(int itemId, List<int> stockIds, List<int> quantities)
    //{
    //    if (itemId <= 0 || stockIds == null || quantities == null || !stockIds.Any())
    //    {
    //        ModelState.AddModelError("", "Please select an item and provide quantities for at least one stock.");
    //        ViewBag.Items = context.items.ToList();
    //        ViewBag.Stocks = context.stocks.ToList();
    //        return View();
    //    }

    //    var item = context.items.FirstOrDefault(i => i.ItemId == itemId);
    //    if (item == null)
    //    {
    //        ModelState.AddModelError("", "Invalid item selection.");
    //        ViewBag.Items = context.items.ToList();
    //        ViewBag.Stocks = context.stocks.ToList();
    //        return View();
    //    }

    //    int totalAddedQuantity = 0;

    //    for (int i = 0; i < stockIds.Count; i++)
    //    {
    //        int stockId = stockIds[i];
    //        int quantityToAdd = quantities[i];

    //        if (quantityToAdd > 0)
    //        {
    //            totalAddedQuantity += quantityToAdd;

    //            var stockItem = context.stockItems.FirstOrDefault(si => si.ItemId == itemId && si.StockId == stockId);
    //            if (stockItem != null)
    //            {
    //                stockItem.Quantity += quantityToAdd; // Add to existing quantity
    //            }
    //            else
    //            {
    //                // If the stock doesn't already have the item, create it
    //                context.stockItems.Add(new StockItem
    //                {
    //                    StockId = stockId,
    //                    ItemId = itemId,
    //                    Quantity = quantityToAdd
    //                });
    //            }
    //        }
    //    }

    //    // Update the total quantity of the item
    //    item.TotalQuantity += totalAddedQuantity;

    //    context.SaveChanges();
    //    return RedirectToAction("Index");
    //}


    [HttpPost]
    public IActionResult Create(int ItemId, List<int> StockIds, List<int> Quantities)
    {
        if (ItemId <= 0 || StockIds == null || Quantities == null || !StockIds.Any())
        {
            ModelState.AddModelError("", "يرجى اختيار العنصر وتحديد الكمية لمخزن واحد على الأقل.");
            ViewBag.Items = context.items.ToList();
            ViewBag.Stocks = context.stocks.ToList();
            return View();
        }

        var item = context.items.FirstOrDefault(i => i.ItemId == ItemId);
        if (item == null)
        {
            ModelState.AddModelError("", "العنصر المحدد غير موجود.");
            ViewBag.Items = context.items.ToList();
            ViewBag.Stocks = context.stocks.ToList();
            return View();
        }

        int totalAddedQuantity = 0;

        // معالجة المخازن والكميات
        for (int i = 0; i < StockIds.Count; i++)
        {
            int stockId = StockIds[i];
            int quantityToAdd = Quantities[i];

            if (quantityToAdd > 0)
            {
                totalAddedQuantity += quantityToAdd;

                var stockItem = context.stockItems.FirstOrDefault(si => si.ItemId == ItemId && si.StockId == stockId);
                if (stockItem != null)
                {
                    stockItem.Quantity += quantityToAdd; // تحديث الكمية
                }
                else
                {
                    // إضافة علاقة جديدة بين المخزن والعنصر
                    context.stockItems.Add(new StockItem
                    {
                        StockId = stockId,
                        ItemId = ItemId,
                        Quantity = quantityToAdd
                    });
                }
            }
        }

        // تحديث الكمية الإجمالية للعنصر
        item.TotalQuantity += totalAddedQuantity;

        context.SaveChanges();

        TempData["SuccessMessage"] = "تمت إضافة الكميات بنجاح للمخازن المحددة.";
        return RedirectToAction("Index");
    }




    [HttpGet]
    public IActionResult AddNewItem()
    {
        ViewBag.Stocks = context.stocks.ToList(); // Populate available stocks
        return View();
    }
    [HttpPost]
    public IActionResult AddNewItem(Item item, List<int> stockIds, List<int> quantities)
    {
        if (string.IsNullOrWhiteSpace(item.ItemName) || item.TotalQuantity <= 0)
        {
            ModelState.AddModelError("", "Item name and total quantity are required.");
            ViewBag.Stocks = context.stocks.ToList(); // Repopulate stocks for the view
            return View();
        }

        // Check if the item already exists
        var existingItem = context.items.FirstOrDefault(i => i.ItemName == item.ItemName);
        if (existingItem != null)
        {
            ModelState.AddModelError("", "Item already exists. Use the Add Quantity form instead.");
            ViewBag.Stocks = context.stocks.ToList(); // Repopulate stocks for the view
            return View();
        }

        // Validate totalQuantity equals the sum of quantities
        int sumOfQuantities = quantities.Where(q => q > 0).Sum();
        if (sumOfQuantities != item.TotalQuantity)
        {
            ModelState.AddModelError("", "The total quantity must equal the sum of quantities assigned to stocks.");
            ViewBag.Stocks = context.stocks.ToList(); // Repopulate stocks for the view
            return View(item);
        }

       
        // Create a new item

        context.items.Add(item);
        context.SaveChanges();

        // Assign quantities to stocks
        for (int i = 0; i < stockIds.Count; i++)
        {
            int stockId = stockIds[i];
            int quantity = quantities[i];

            if (quantity > 0)
            {
                context.stockItems.Add(new StockItem
                {
                    StockId = stockId,
                    ItemId = item.ItemId,
                    Quantity = quantity
                });
            }
        }
        context.SaveChanges();

        return RedirectToAction("Index");
    }



}