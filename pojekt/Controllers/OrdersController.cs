using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pojekt.Models;
using System.Security.Claims;

namespace pojekt.Controllers
{
    public class OrdersController : Controller
    {
        private readonly WarehouseContext _context;

        public OrdersController(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Pobierz userId z sesji
            string userIdString = HttpContext.Session.GetString("UserId");
            string userRole = null;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // Jeśli użytkownik nie jest zalogowany lub userId nie jest poprawne, przekieruj na stronę logowania
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var currentUser = await _context.Users
                    .Where(u => u.ID == userId)
                    .Select(u => new { u.Role })
                    .FirstOrDefaultAsync();

                userRole = currentUser?.Role;
            }

            var userOrders = await _context.Orders
                         .Where(o => o.UserId == userId)
                         .Include(o => o.Product) // Dołącz informacje o produktach
                         .ToListAsync();

            if (userRole == "Admin")
            {
                userOrders = await _context.Orders.ToListAsync();
                return View(userOrders);
            }


            return View(userOrders);


        }


    }
}

