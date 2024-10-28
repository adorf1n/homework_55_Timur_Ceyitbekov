using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(p => p.Category).Include(p => p.Brand).ToListAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
        ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name");
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
    {
        Console.WriteLine("POST Create метод вызван.");
        Console.WriteLine($"Название: {product.Name}");
        Console.WriteLine($"Цена: {product.Price}");
        Console.WriteLine($"Категория ID: {product.CategoryId}");
        Console.WriteLine($"Бренд ID: {product.BrandId}");

        if (ImageFile != null && ImageFile.Length > 0)
        {
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
            Console.WriteLine($"Расширение файла: {ext}");

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                Console.WriteLine("Ошибка: Неверный формат файла.");
                ModelState.AddModelError("ImageFile", "Допустимые форматы изображений: .jpg, .jpeg, .png.");
            }
            else if (ImageFile.Length > 5 * 1024 * 1024)
            {
                Console.WriteLine("Ошибка: Размер файла превышает 5 МБ.");
                ModelState.AddModelError("ImageFile", "Размер файла не должен превышать 5 МБ.");
            }
            else
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                product.ImagePath = "/images/" + uniqueFileName;
                Console.WriteLine("Файл успешно сохранен.");
            }
        }
        else
        {
            Console.WriteLine("Ошибка: Изображение не было загружено.");
            ModelState.AddModelError("ImageFile", "Пожалуйста, загрузите изображение товара.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            Console.WriteLine("Товар успешно сохранен в базе данных.");
            return RedirectToAction(nameof(Index));
        }

        Console.WriteLine("ModelState не валиден. Ошибки валидации:");
        foreach (var modelState in ModelState)
        {
            foreach (var error in modelState.Value.Errors)
            {
                Console.WriteLine($"Ошибка: {error.ErrorMessage}");
            }
        }

        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
        return View(product);
    }






    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
        return View(product);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}