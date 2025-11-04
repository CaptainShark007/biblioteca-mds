using Microsoft.AspNetCore.Mvc;
using trabajoMetodologiaDSistemas.Data;
using trabajoMetodologiaDSistemas.Models;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class PrestamoController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                _context.Prestamos.Add(prestamo);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(prestamo);
        }
    }
}
