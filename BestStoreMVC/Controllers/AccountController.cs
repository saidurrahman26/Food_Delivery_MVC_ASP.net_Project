using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BestStoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = model.Password,
                IsAdmin = (model.Email == "saidurrahmansajid@gmail.com" || model.Email == "tawabahmednafi@gmail.com")
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if email exists
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid email");
                return View(model);
            }

            // Check if password matches
            if (user.PasswordHash != model.Password)
            {
                ModelState.AddModelError("Password", "Invalid password");
                return View(model);
            }

            // Login success - set session
            HttpContext.Session.SetString("IsAdmin", user.IsAdmin ? "true" : "false");

            // ✅ Redirect based on role
            if (user.IsAdmin)
                return RedirectToAction("Dashboard", "Admin");  // Admin → Dashboard

            return RedirectToAction("ViewProducts", "Products"); // User → View Menu
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

    }
}
