using Microsoft.AspNetCore.Mvc;
using trabajoMetodologiaDSistemas.Data;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class PrestamoController : Controller
    {
        public IActionResult Index()
        {
            var prestamos = _context.Prestamos.ToList();
            return View(prestamos);
        }
    }
}
