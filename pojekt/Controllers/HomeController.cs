using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
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
                // Pobierz rol� u�ytkownika na podstawie userId
                var currentUser = await _context.Users
                    .Where(u => u.ID == userId)
                    .Select(u => new { u.Role })
                    .FirstOrDefaultAsync();

                userRole = currentUser?.Role;
            }

            return View((userRole, products));
        }

        // Obs�uga sk�adania zam�wienia
        [HttpPost]
        public async Task<IActionResult> Order(int productId, int quantity)
        {
            // Pobierz userId z sesji
            string userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(); // U�ytkownik nie jest zalogowany
            }

            // Pobierz produkt
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Produkt nie zosta� znaleziony.");
            }

            // Sprawd�, czy w magazynie jest wystarczaj�ca ilo�� produkt�w
            if (int.TryParse(product.Quantity, out int stockQuantity) && stockQuantity < quantity)
            {
                return BadRequest("Nie ma wystarczaj�cej ilo�ci produktu w magazynie.");
            }

            // Zmniejsz liczb� produkt�w w magazynie
            product.Quantity = (stockQuantity - quantity).ToString();

            // Oblicz ca�kowit� cen�
            var totalPrice = product.Price * quantity;

            // Utw�rz nowe zam�wienie
            var order = new OrderModel
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                Status = "W trakcie realizacji"
            };

            // Zapisz zmiany w bazie danych
            _context.Orders.Add(order);
            _context.Products.Update(product); // Aktualizuj dane produktu
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
