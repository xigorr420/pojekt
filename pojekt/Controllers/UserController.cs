using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using pojekt.Models;

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

            return View(user);
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
            return View(user);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Usuwa wszystkie dane sesji
            return RedirectToAction("Index", "Home"); // Przekierowanie po wylogowaniu
        }
    }
}
