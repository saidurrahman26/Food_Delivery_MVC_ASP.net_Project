using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            // Optional: You can add a check to make sure only admin can access
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Index", "Home");

            return View();
        }
    }
}
