using Microsoft.AspNetCore.Mvc;

namespace Anket.Views.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
