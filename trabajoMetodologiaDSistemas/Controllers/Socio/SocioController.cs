using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trabajoMetodologiaDSistemas.Data;
using trabajoMetodologiaDSistemas.Models;
using trabajoMetodologiaDSistemas.Dtos;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public class SocioController : Controller
    {
        private readonly AppDbContext _context;

        public SocioController(AppDbContext context)
        {
            _context = context;
        }

        #region Registro de Nuevos Socios (Según Diagrama de Actividad)

        // GET: /Socio/Registrarse
        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        // POST: /Socio/Registrarse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrarse(Socio socio)
        {
            // Paso del diagrama: ¿DNI ya existe?
            if (_context.Socios.Any(s => s.Dni == socio.Dni))
            {
                ViewBag.Error = "El DNI ya está registrado. No se puede crear un nuevo socio con este DNI.";
                return View(socio);
            }

            if (!ModelState.IsValid)
            {
                return View(socio);
            }

            try
            {
                // Bibliotecario registra los datos
                socio.NumeroSocio = "TEMP";
                _context.Socios.Add(socio);
                _context.SaveChanges();

                // Sistema genera número único de socio
                socio.NumeroSocio = $"NS{socio.IdSocio:D6}";
                _context.SaveChanges();

                // Alta de socio exitosa
                ViewBag.NumeroSocio = socio.NumeroSocio;
                ViewBag.NombreSocio = socio.Nombre;
                ViewBag.DniSocio = socio.Dni;

                return View("RegistroExitoso");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al registrar el socio: {ex.Message}";
                return View(socio);
            }
        }

        #endregion

        #region Consultar Préstamos (Según Diagrama de Clases)

        // GET: /Socio/ConsultarPrestamos
        [HttpGet]
        public IActionResult ConsultarPrestamos()
        {
            return View();
        }

        // POST: /Socio/ConsultarPrestamos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConsultarPrestamos(string numeroSocio)
        {
            if (string.IsNullOrWhiteSpace(numeroSocio))
            {
                ViewBag.Error = "Debe ingresar el número de socio.";
                return View();
            }

            var socio = _context.Socios.FirstOrDefault(s => s.NumeroSocio == numeroSocio);
            if (socio == null)
            {
                ViewBag.Error = "El número de socio no es válido.";
                return View();
            }

            var prestamos = _context.Prestamos
                .Where(p => p.IdSocio == socio.IdSocio)
                .Include(p => p.Libro)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToList();

            ViewBag.NumeroSocio = numeroSocio;
            ViewBag.NombreSocio = socio.Nombre;
            ViewBag.Prestamos = prestamos;
            return View();
        }

        #endregion

        #region Consultar Multas (Según Diagrama de Clases)

        // GET: /Socio/ConsultarMultas
        [HttpGet]
        public IActionResult ConsultarMultas()
        {
            return View();
        }

        // POST: /Socio/ConsultarMultas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConsultarMultas(string numeroSocio)
        {
            if (string.IsNullOrWhiteSpace(numeroSocio))
            {
                ViewBag.Error = "Debe ingresar el número de socio.";
                return View();
            }

            var socio = _context.Socios.FirstOrDefault(s => s.NumeroSocio == numeroSocio);
            if (socio == null)
            {
                ViewBag.Error = "El número de socio no es válido.";
                return View();
            }

            var multas = _context.Multas
                .Where(m => m.IdSocio == socio.IdSocio)
                .Include(m => m.Libro)
                .OrderByDescending(m => m.Fecha)
                .ToList();

            ViewBag.NumeroSocio = numeroSocio;
            ViewBag.NombreSocio = socio.Nombre;
            ViewBag.Multas = multas;
            return View();
        }

        #endregion
    }
}