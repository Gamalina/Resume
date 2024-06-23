using Microsoft.AspNetCore.Mvc;

namespace Resume.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
