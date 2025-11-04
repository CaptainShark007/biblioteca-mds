using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trabajoMetodologiaDSistemas.Data;
using trabajoMetodologiaDSistemas.Models;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class PrestamoController : Controller
    {
        #region Devolución de Libro (Según Diagrama de Actividades)

        // GET: /Prestamo/Devolver
        [HttpGet]
        public IActionResult Devolver()
        {
            return View();
        }

        // POST: /Prestamo/Devolver - Workflow: Buscar Préstamos -> Verificar Estado Físico -> Finalizar o Registrar Multa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Devolver(string? numeroSocio, int? prestamoId, string? estadoFisico, string? accion)
        {
            // Paso 1: Buscar préstamos activos del socio
            if (accion == "buscarPrestamos" || (string.IsNullOrEmpty(accion) && !string.IsNullOrWhiteSpace(numeroSocio) && !prestamoId.HasValue))
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

                var prestamosActivos = _context.Prestamos
                    .Where(p => p.IdSocio == socio.IdSocio && p.FechaDevolucion == null)
                    .Include(p => p.Libro)
                    .ToList();

                if (!prestamosActivos.Any())
                {
                    ViewBag.Error = "El socio no tiene préstamos activos.";
                    return View();
                }

                // Filtrar préstamos que tengan libro
                prestamosActivos = prestamosActivos.Where(p => p.Libro != null).ToList();

                ViewBag.NumeroSocio = numeroSocio;
                ViewBag.NombreSocio = socio.Nombre;
                ViewBag.PrestamosActivos = prestamosActivos;
                return View();
            }

            // Paso 2: Verificar estado físico
            if (accion == "verificarEstado" && prestamoId.HasValue)
            {
                var prestamo = _context.Prestamos
                    .Include(p => p.Libro)
                    .Include(p => p.Socio)
                    .FirstOrDefault(p => p.IdPrestamo == prestamoId.Value);

                if (prestamo == null)
                {
                    ViewBag.Error = "Préstamo no encontrado.";
                    return View();
                }

                // Asegurarse de que Libro y Socio no sean null
                if (prestamo.Libro == null || prestamo.Socio == null)
                {
                    ViewBag.Error = "Error: Datos del préstamo incompletos.";
                    return View();
                }

                ViewBag.PrestamoSeleccionado = prestamo;
                ViewBag.NumeroSocio = prestamo.Socio.NumeroSocio;
                return View();
            }

            // Paso 3: Finalizar préstamo o registrar multa
            if (accion == "finalizarDevolucion" && prestamoId.HasValue && !string.IsNullOrEmpty(estadoFisico))
            {
                var prestamo = _context.Prestamos
                    .Include(p => p.Libro)
                    .Include(p => p.Socio)
                    .FirstOrDefault(p => p.IdPrestamo == prestamoId.Value);

                if (prestamo == null)
                {
                    ViewBag.Error = "Préstamo no encontrado.";
                    return View();
                }

                if (prestamo.Libro == null)
                {
                    ViewBag.Error = "Error: Libro no encontrado.";
                    return View();
                }

                try
                {
                    // Bibliotecario registra devolución
                    prestamo.RegistrarDevolucion();

                    // Sistema cambia estado del libro
                    prestamo.Libro.CantidadDisponible++;
                    prestamo.Libro.Estado = "Disponible";

                    // ¿Libro devuelto en buen estado?
                    if (estadoFisico == "malo")
                    {
                        // Bibliotecario registra multa al socio
                        var multa = new Multa
                        {
                            IdSocio = prestamo.IdSocio,
                            IdLibro = prestamo.IdLibro,
                            Fecha = DateTime.Now,
                            DiasRestringido = 15,
                            Monto = 0,
                            Descripcion = "Libro devuelto con daños",
                            Estado = "Activa"
                        };
                        _context.Multas.Add(multa);
                    }

                    // Verificar si hay retraso
                    if (prestamo.FechaDevolucion.HasValue && 
                        prestamo.FechaDevolucion > prestamo.FechaCorrespondienteDevolucion)
                    {
                        var diasRetraso = (prestamo.FechaDevolucion.Value.Date - prestamo.FechaCorrespondienteDevolucion.Date).Days;
                        var multa = new Multa
                        {
                            IdSocio = prestamo.IdSocio,
                            IdLibro = prestamo.IdLibro,
                            Fecha = DateTime.Now,
                            DiasRestringido = diasRetraso,
                            Monto = 0,
                            Descripcion = $"Devolución con {diasRetraso} días de retraso",
                            Estado = "Activa"
                        };
                        _context.Multas.Add(multa);
                    }

                    _context.SaveChanges();

                    ViewBag.Success = $"Devolución registrada exitosamente. Libro: '{prestamo.Libro.Titulo}'.";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error al registrar la devolución: {ex.Message}";
                    return View();
                }
            }

            return View();
        }

        #endregion
    }
}
