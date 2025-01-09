using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using pojekt.Models;

namespace pojekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly WarehouseContext _context;

        public HomeController(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            string userIdString = HttpContext.Session.GetString("UserId"); // Pobierz userId jako string
            string userRole = null;

            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId)) // Konwersja na int
            {
                // Pobierz rolê u¿ytkownika na podstawie userId
                var currentUser = await _context.Users
                    .Where(u => u.ID == userId)
                    .Select(u => new { u.Role })
                    .FirstOrDefaultAsync();

                userRole = currentUser?.Role;
            }

            return View((userRole, products));
        }

        [HttpPost]
        public async Task<IActionResult> Order(int productId, int quantity)
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Produkt nie zosta³ znaleziony.");
            }

            if (int.TryParse(product.Quantity, out int stockQuantity) && stockQuantity < quantity)
            {
                return BadRequest("Nie ma wystarczaj¹cej iloœci produktu w magazynie.");
            }

            product.Quantity = (stockQuantity - quantity).ToString();
            var totalPrice = product.Price * quantity;

            var order = new OrderModel
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                Status = "W trakcie realizacji"
            };

            _context.Orders.Add(order);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductModel updatedProduct)
        {
            var product = await _context.Products.FindAsync(updatedProduct.ProductId);
            if (product == null)
            {
                return NotFound("Produkt nie zosta³ znaleziony.");
            }

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Category = updatedProduct.Category;
            product.Quantity = updatedProduct.Quantity;
            product.Price = updatedProduct.Price;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Produkt nie zosta³ znaleziony.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductModel newProduct)
        {
            if (!ModelState.IsValid)
            {
                // Logowanie b³êdów walidacji w konsoli serwera
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"B³¹d walidacji: {error.ErrorMessage}");
                }
                return View(newProduct);
            }

            Console.WriteLine($"Dodawanie produktu: {newProduct.Name}, Iloœæ: {newProduct.Quantity}, Cena: {newProduct.Price}");

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }

}

