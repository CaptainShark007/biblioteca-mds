using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trabajoMetodologiaDSistemas.Data;
using trabajoMetodologiaDSistemas.Models;
using trabajoMetodologiaDSistemas.Dtos;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public class BibliotecarioController : Controller
    {
        private readonly AppDbContext _context;

        public BibliotecarioController(AppDbContext context)
        {
            _context = context;
        }

        #region Panel Principal

        // GET: /Bibliotecario
        [HttpGet]
        public IActionResult Index()
        {
            var estadisticas = new
            {
                TotalSocios = _context.Socios.Count(),
                TotalLibros = _context.Libros.Count(),
                PrestamosActivos = _context.Prestamos.Count(p => p.FechaDevolucion == null),
                NuevosSocios = _context.Socios.OrderByDescending(s => s.IdSocio).Take(5).Count()
            };

            ViewBag.Estadisticas = estadisticas;
            return View();
        }

        #endregion

        #region Gestión Completa de Socios

        // GET: /Bibliotecario/Socios
        [HttpGet]
        public IActionResult Socios(string filtro = "todos")
        {
            IQueryable<Socio> query = _context.Socios;

            var socios = filtro switch
            {
                "nuevos" => query.OrderByDescending(s => s.IdSocio).Take(5).ToList(),
                "activos" => query.Where(s => _context.Prestamos.Any(p => p.IdSocio == s.IdSocio && p.FechaDevolucion == null)).ToList(),
                _ => query.OrderBy(s => s.NumeroSocio).ToList()
            };

            ViewBag.Filtro = filtro;
            ViewBag.TotalSocios = _context.Socios.Count();
            return View(socios);
        }

        // GET: /Bibliotecario/Socios/Detalles/{id}
        [HttpGet]
        public IActionResult DetallesSocio(int id)
        {
            var socio = _context.Socios
                .Include(s => s.Prestamos)
                    .ThenInclude(p => p.Libro)
                .Include(s => s.Multas)
                .FirstOrDefault(s => s.IdSocio == id);

            if (socio == null)
            {
                return NotFound();
            }

            return View(socio);
        }

        #endregion

        #region Gestión de Préstamos y Devoluciones

        // GET: /Bibliotecario/Prestamos
        [HttpGet]
        public IActionResult Prestamos(string estado = "activos")
        {
            IQueryable<Prestamo> query = _context.Prestamos
                .Include(p => p.Socio)
                .Include(p => p.Libro);

            var prestamos = estado switch
            {
                "activos" => query.Where(p => p.FechaDevolucion == null)
                                 .OrderByDescending(p => p.FechaPrestamo)
                                 .ToList(),
                "devueltos" => query.Where(p => p.FechaDevolucion != null)
                                   .OrderByDescending(p => p.FechaDevolucion)
                                   .ToList(),
                "atrasados" => query.Where(p => p.FechaDevolucion == null &&
                                               p.FechaCorrespondienteDevolucion < DateTime.Now)
                                   .OrderBy(p => p.FechaCorrespondienteDevolucion)
                                   .ToList(),
                _ => query.OrderByDescending(p => p.FechaPrestamo).ToList()
            };

            ViewBag.Estado = estado;
            ViewBag.TotalPrestamos = prestamos.Count;
            return View(prestamos);
        }

        // GET: /Bibliotecario/RegistrarDevolucion
        [HttpGet]
        public IActionResult RegistrarDevolucion()
        {
            return View();
        }

        // POST: /Bibliotecario/RegistrarDevolucion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarDevolucion(string tipoBusqueda, string identificador, int? prestamoSeleccionado)
        {
            if (prestamoSeleccionado.HasValue)
            {
                return ProcesarDevolucion(prestamoSeleccionado.Value);
            }

            return BuscarPrestamosActivos(tipoBusqueda, identificador);
        }

        private IActionResult ProcesarDevolucion(int prestamoId)
        {
            try
            {
                var prestamo = _context.Prestamos
                    .Include(p => p.Socio)
                    .Include(p => p.Libro)
                    .FirstOrDefault(p => p.IdPrestamo == prestamoId);

                if (prestamo == null)
                {
                    ViewBag.Error = "No se encontró el préstamo seleccionado.";
                    return View("RegistrarDevolucion");
                }

                if (prestamo.FechaDevolucion.HasValue)
                {
                    ViewBag.Error = "Este préstamo ya fue devuelto.";
                    return View("RegistrarDevolucion");
                }

                prestamo.FechaDevolucion = DateTime.Now;
                prestamo.Libro.CantidadDisponible++;

                string mensaje = "Devolución registrada correctamente.";

                // Verificar si genera multa
                if (prestamo.FechaDevolucion > prestamo.FechaCorrespondienteDevolucion)
                {
                    var diasAtraso = (prestamo.FechaDevolucion.Value - prestamo.FechaCorrespondienteDevolucion).Days;
                    var multa = new Multa
                    {
                        IdSocio = prestamo.IdSocio,
                        IdLibro = prestamo.IdLibro,
                        Fecha = DateTime.Now,
                        DiasRestringido = Math.Max(3, diasAtraso),
                        Descripcion = $"Devolución atrasada: {diasAtraso} día(s) de retraso"
                    };

                    _context.Multas.Add(multa);
                    mensaje = $"Devolución registrada. Se generó multa de {multa.DiasRestringido} días por atraso.";
                }

                _context.SaveChanges();
                ViewBag.Exito = mensaje;
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al registrar devolución: {ex.Message}";
            }

            return View("RegistrarDevolucion");
        }

        private IActionResult BuscarPrestamosActivos(string tipoBusqueda, string identificador)
        {
            if (string.IsNullOrWhiteSpace(tipoBusqueda) || string.IsNullOrWhiteSpace(identificador))
            {
                ViewBag.Error = "Debe seleccionar el tipo de búsqueda e ingresar un valor.";
                return View("RegistrarDevolucion");
            }

            Socio socio = null;

            if (tipoBusqueda == "dni")
            {
                socio = _context.Socios.FirstOrDefault(s => s.Dni == identificador);
            }
            else if (tipoBusqueda == "numSocio")
            {
                socio = _context.Socios.FirstOrDefault(s => s.NumeroSocio == identificador);
            }

            if (socio == null)
            {
                ViewBag.Error = $"No se encontró un socio con el {tipoBusqueda} proporcionado.";
                return View("RegistrarDevolucion");
            }

            var prestamos = _context.Prestamos
                .Where(p => p.IdSocio == socio.IdSocio && p.FechaDevolucion == null)
                .Include(p => p.Libro)
                .Select(p => new
                {
                    p.IdPrestamo,
                    p.Libro.Titulo,
                    p.FechaPrestamo,
                    p.FechaCorrespondienteDevolucion,
                    DiasRestante = (p.FechaCorrespondienteDevolucion - DateTime.Now).Days
                })
                .ToList();

            if (!prestamos.Any())
            {
                ViewBag.Error = "No hay préstamos activos para este socio.";
                return View("RegistrarDevolucion");
            }

            ViewBag.Prestamos = prestamos;
            ViewBag.TipoBusqueda = tipoBusqueda;
            ViewBag.Identificador = identificador;
            ViewBag.NumeroSocio = socio.NumeroSocio;
            ViewBag.NombreSocio = socio.Nombre;

            return View("RegistrarDevolucion");
        }

        #endregion

        #region Gestión de Multas

        // GET: /Bibliotecario/Multas
        [HttpGet]
        public IActionResult Multas(string estado = "activas")
        {
            IQueryable<Multa> query = _context.Multas
                .Include(m => m.Socio)
                .Include(m => m.Libro);

            var multas = estado switch
            {
                "activas" => query.Where(m => m.Fecha.AddDays(m.DiasRestringido) > DateTime.Now)
                                 .OrderByDescending(m => m.Fecha)
                                 .ToList(),
                "expiradas" => query.Where(m => m.Fecha.AddDays(m.DiasRestringido) <= DateTime.Now)
                                   .OrderByDescending(m => m.Fecha)
                                   .ToList(),
                _ => query.OrderByDescending(m => m.Fecha).ToList()
            };

            ViewBag.Estado = estado;
            ViewBag.TotalMultas = multas.Count;
            return View(multas);
        }

        // POST: /Bibliotecario/Multas/Eliminar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarMulta(int id)
        {
            try
            {
                var multa = _context.Multas.Find(id);
                if (multa == null)
                {
                    return NotFound();
                }

                _context.Multas.Remove(multa);
                _context.SaveChanges();

                TempData["MensajeExito"] = "Multa eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al eliminar la multa: {ex.Message}";
            }

            return RedirectToAction(nameof(Multas));
        }

        #endregion
    }
}