using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using pojekt.Models;
using Microsoft.EntityFrameworkCore;

namespace pojekt.Controllers
{
    public class UserController : Controller
    {
        private readonly WarehouseContext _context;

        public UserController(WarehouseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home"); // Przekierowanie na stronę główną lub inną stronę
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (ModelState.IsValid)
            {
                // Ustaw domyślną rolę, jeśli nie została podana
                user.Role = "User";

                if (!_context.Users.Any(u => u.Email == user.Email))
                {
                    try
                    {
                        _context.Users.Add(user);
                        _context.SaveChanges();

                        // Po zapisaniu użytkownika, dodaj dane do tabeli UserDetails
                        var userDetails = new UserDetailsModel
                        {
                            UserId = user.ID, // Przypisanie UserId z nowo utworzonego użytkownika
                            City = "Default", // Można ustawić domyślne wartości
                            Street = "Default",
                            HouseNumber = "Default",
                            PhoneNumber = "Default"
                        };

                        // Dodanie danych użytkownika do tabeli UserDetails
                        _context.UserDetails.Add(userDetails);
                        _context.SaveChanges();

                        TempData["Success"] = "User registered successfully!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while saving user: {ex.Message}");
                        ModelState.AddModelError("", "An error occurred while saving the user.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email already exi sts.");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine(string.Join(", ", errors));
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home"); // Przekierowanie na stronę główną lub inną stronę
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (existingUser != null)
            {
                HttpContext.Session.SetString("UserId", existingUser.ID.ToString());
                HttpContext.Session.SetString("Username", existingUser.Email);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid username or password.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Usuwa wszystkie dane sesji
            return RedirectToAction("Index", "Home"); // Przekierowanie po wylogowaniu
        }

        public async Task<IActionResult> Update()
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                return RedirectToAction("Login", "User"); // Przekierowanie na stronę logowania
            }

            if (int.TryParse(userIdString, out int userId))
            {
                // Sprawdzanie, czy użytkownik już ma dane
                var userDetails = await _context.UserDetails
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (userDetails == null)
                {
                    // Jeśli użytkownik nie ma danych, wyświetl formularz do dodania danych
                    return View(new UserDetailsModel { UserId = userId });
                }

                // Jeśli użytkownik ma dane, wyświetl formularz do edycji
                return View(userDetails);
            }

            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserDetailsModel userDetails)
        {
            if (ModelState.IsValid)
            {
                var existingUserDetails = await _context.UserDetails
                    .FirstOrDefaultAsync(u => u.UserId == userDetails.UserId);

                if (existingUserDetails == null)
                {
                    return NotFound(); // Jeśli użytkownik nie istnieje
                }

                // Aktualizacja danych użytkownika
                existingUserDetails.City = userDetails.City;
                existingUserDetails.Street = userDetails.Street;
                existingUserDetails.HouseNumber = userDetails.HouseNumber;
                existingUserDetails.PhoneNumber = userDetails.PhoneNumber;

                // Zapisz zmiany w bazie danych
                _context.Update(existingUserDetails);
                await _context.SaveChangesAsync();

                // Przekierowanie na stronę z listą użytkowników lub inną stronę
                return RedirectToAction("Index", "Home"); // Możesz zmienić na odpowiednią stronę
            }

            // Jeśli ModelState jest nieprawidłowy, zwróć ponownie formularz
            return View(userDetails);
        }
    }
}
