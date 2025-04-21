using InventaryApp.data;
using InventaryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventaryApp.Controllers
{
    public class ProductsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchName, string searchCategory)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(p => p.Name.Contains(searchName));

            if (!string.IsNullOrWhiteSpace(searchCategory))
                query = query.Where(p => p.Category == searchCategory);

            var products = await query.ToListAsync();
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
                return NotFound();

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
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Products/Buy/5
        public async Task<IActionResult> Buy(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.Stock++;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Producto '{product.Name}' comprado correctamente. Nuevo stock: {product.Stock}.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Products/Sell/5
        public async Task<IActionResult> Sell(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null && product.Stock > 0)
            {
                product.Stock--;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Producto '{product.Name}' vendido correctamente. Nuevo stock: {product.Stock}.";
            }
            else
            {
                TempData["ErrorMessage"] = $"No hay stock disponible para vender el producto '{product.Name}'.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
