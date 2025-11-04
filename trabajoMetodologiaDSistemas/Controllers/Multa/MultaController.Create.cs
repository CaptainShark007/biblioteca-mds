using Microsoft.AspNetCore.Mvc;
using trabajoMetodologiaDSistemas.Data;
using trabajoMetodologiaDSistemas.Models;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class MultaController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Multa multa)
        {
            if (ModelState.IsValid)
            {
                _context.Multas.Add(multa);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(multa);
        }
    }
}
