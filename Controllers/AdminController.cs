using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
