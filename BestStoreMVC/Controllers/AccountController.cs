using BestStoreMVC.Models; // Ensure this matches your project namespace
using BestStoreMVC.Services; // If your ApplicationDbContext is under Services
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    // Constructor to inject ApplicationDbContext
    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Account/Register
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Save user to the database
        var user = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = model.Password // (optional: hash the password later)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Registration successful!";
        return RedirectToAction("Index", "Home");
    }
}
