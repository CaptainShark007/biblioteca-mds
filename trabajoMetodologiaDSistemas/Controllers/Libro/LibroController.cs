using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trabajoMetodologiaDSistemas.Data;
using trabajoMetodologiaDSistemas.Models;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public class LibroController : Controller
    {
        private readonly AppDbContext _context;

        public LibroController(AppDbContext context)
        {
            _context = context;
        }

        // Listado principal
        public IActionResult Index()
        {
            var libros = _context.Libros.ToList();
            return View(libros);
        }

        // Formulario crear
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Libros.Add(libro);
                _context.SaveChanges();
                TempData["MensajeExito"] = "Libro agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        // Formulario editar
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var libro = _context.Libros.Find(id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Libros.Update(libro);
                _context.SaveChanges();
                TempData["MensajeExito"] = "Libro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var libro = _context.Libros.Find(id);
            if (libro == null)
                return NotFound();

            _context.Libros.Remove(libro);
            _context.SaveChanges();
            TempData["MensajeExito"] = "Libro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
