using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trabajoMetodologiaDSistemas.Data;
using trabajoMetodologiaDSistemas.Models;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class PrestamoController : Controller
    {
        #region Solicitar Préstamo (Según Diagrama de Actividad)

        // GET: /Prestamo/Solicitar
        [HttpGet]
        public IActionResult Solicitar()
        {
            return View();
        }

        // POST: /Prestamo/Solicitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Solicitar(string numeroSocio, int? idLibro, string accion)
        {
            // Paso 1: Buscar socio
            if (accion == "buscarSocio" || (string.IsNullOrEmpty(accion) && !string.IsNullOrWhiteSpace(numeroSocio) && idLibro == null))
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

                // Verificar multas activas (¿Socio tiene multas impagas?)
                var multaActiva = _context.Multas
                    .Where(m => m.IdSocio == socio.IdSocio && m.Fecha.AddDays(m.DiasRestringido) > DateTime.Now)
                    .FirstOrDefault();

                if (multaActiva != null)
                {
                    var fechaFin = multaActiva.Fecha.AddDays(multaActiva.DiasRestringido);
                    ViewBag.Error = $"Préstamo rechazado. El socio tiene una multa activa hasta el {fechaFin:dd/MM/yyyy}.";
                    return View();
                }

                // Mostrar libros disponibles (¿Libro está disponible?)
                var librosDisponibles = _context.Libros
                    .Where(l => l.CantidadDisponible > 0)
                    .ToList();

                if (!librosDisponibles.Any())
                {
                    ViewBag.Error = "No hay libros disponibles en este momento.";
                    return View();
                }

                ViewBag.NumeroSocio = numeroSocio;
                ViewBag.NombreSocio = socio.Nombre;
                ViewBag.LibrosDisponibles = librosDisponibles;
                return View();
            }

            // Paso 2: Confirmar préstamo
            if (accion == "confirmarPrestamo" && idLibro.HasValue)
            {
                var socio = _context.Socios.FirstOrDefault(s => s.NumeroSocio == numeroSocio);
                if (socio == null)
                {
                    ViewBag.Error = "Número de socio inválido.";
                    return View();
                }

                var libro = _context.Libros.Find(idLibro.Value);
                if (libro == null || !libro.EstaDisponible())
                {
                    ViewBag.Error = "El libro seleccionado no está disponible.";
                    return View();
                }

                try
                {
                    // Bibliotecario registra préstamo
                    var prestamo = new Prestamo
                    {
                        IdSocio = socio.IdSocio,
                        IdLibro = libro.Id
                    };
                    prestamo.RegistrarPrestamo(); // Método del diagrama de clases
                    
                    _context.Prestamos.Add(prestamo);

                    // Sistema cambia estado del libro
                    libro.CantidadDisponible--;
                    libro.Estado = libro.CantidadDisponible > 0 ? "Disponible" : "No Disponible";

                    _context.SaveChanges();

                    ViewBag.Success = $"Préstamo registrado exitosamente. Libro: '{libro.Titulo}'. Debe devolverlo antes del {prestamo.FechaCorrespondienteDevolucion:dd/MM/yyyy}.";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error al registrar el préstamo: {ex.Message}";
                    return View();
                }
            }

            return View();
        }

        #endregion
    }
}
