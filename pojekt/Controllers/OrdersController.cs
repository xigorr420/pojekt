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

            // Sprawdź, czy userId jest poprawne (sprawdzamy sesję)
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // Jeśli użytkownik nie jest zalogowany lub userId nie jest poprawne, przekieruj na stronę logowania
                return RedirectToAction("Login", "Account");
            }

            // Pobierz rolę użytkownika na podstawie userId
            var currentUser = await _context.Users
                .Where(u => u.ID == userId)
                .Select(u => new { u.Role })
                .FirstOrDefaultAsync();

            userRole = currentUser?.Role;

            // Pobierz zamówienia użytkownika
            var userOrders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Product) // Dołącz informacje o produktach
                .ToListAsync();

            // Jeśli użytkownik jest adminem, pobierz wszystkie zamówienia
            if (userRole == "Admin")
            {
                userOrders = await _context.Orders
                    .Include(o => o.Product) // Dołącz informacje o produktach
                    .ToListAsync(); // Pobierz wszystkie zamówienia (nie tylko tego użytkownika)
            }

            // Zwróć widok z listą zamówień
            return View((userRole, userOrders));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Pobierz zamówienie z bazy danych
            var order = await _context.Orders
                .Include(o => o.Product) // Załaduj szczegóły produktu
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Zamówienie nie zostało znalezione.");
            }

         

            return View(order); // Zwróć widok do edycji zamówienia
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OrderModel updatedOrder)
        {
            // Pobierz istniejące zamówienie z bazy danych
            var order = await _context.Orders.FindAsync(updatedOrder.Id);

            if (order == null)
            {
                return NotFound("Zamówienie nie zostało znalezione.");
            }

            // Zaktualizuj dane zamówienia
            order.Quantity = updatedOrder.Quantity;
            order.TotalPrice = updatedOrder.TotalPrice; 
            order.Status = updatedOrder.Status;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            string userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound("Zamówienie nie zostało znalezione.");
            }

            // Sprawdź, czy zamówienie należy do użytkownika
            if (order.UserId != userId)
            {
                return Forbid();
            }

            // Oznacz zamówienie jako anulowane
            order.Status = "Anulowane";

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}

