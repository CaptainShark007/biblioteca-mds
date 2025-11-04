using Microsoft.AspNetCore.Mvc;
using trabajoMetodologiaDSistemas.Data;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class MultaController : Controller
    {
        public IActionResult Index()
        {
            var multas = _context.Multas.ToList();
            return View(multas);
        }
    }
}
