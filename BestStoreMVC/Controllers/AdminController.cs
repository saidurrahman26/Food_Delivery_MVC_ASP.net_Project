using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
           
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Index", "Home");

            return View();
        }
    }
}
