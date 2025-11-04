using Microsoft.AspNetCore.Mvc;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
