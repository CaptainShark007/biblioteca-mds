using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trabajoMetodologiaDSistemas.Data;
using trabajoMetodologiaDSistemas.Models;

namespace trabajoMetodologiaDSistemas.Controllers
{
    public partial class PrestamoController : Controller
    {
      #region Préstamo de Libro (Según Diagrama de Actividades)

    // GET: /Prestamo/Crear
      [HttpGet]
public IActionResult Crear()
    {
   return View();
        }

        // POST: /Prestamo/Crear - Workflow del diagrama: Validar Socio -> Verificar Estado Libro -> Registrar Préstamo
 [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string numeroSocio, string tituloLibro, int? idLibro, int? idSocio, string accion)
        {
            // Paso 1: Validar Socio
    if (accion == "validarSocio" || (string.IsNullOrEmpty(accion) && !string.IsNullOrWhiteSpace(numeroSocio)))
      {
         if (string.IsNullOrWhiteSpace(numeroSocio))
 {
    ViewBag.Error = "Debe ingresar el número de socio.";
          return View();
            }

       var socio = _context.Socios.FirstOrDefault(s => s.NumeroSocio == numeroSocio);
    if (socio == null)
                {
                    ViewBag.Error = "El número de socio no es válido. Socio no encontrado.";
      return View();
  }

   // Verificar si tiene multa activa
     var multaActiva = _context.Multas
         .Where(m => m.IdSocio == socio.IdSocio && m.Fecha.AddDays(m.DiasRestringido) > DateTime.Now)
         .FirstOrDefault();

   if (multaActiva != null)
             {
      var fechaFinMulta = multaActiva.Fecha.AddDays(multaActiva.DiasRestringido);
    ViewBag.Error = $"El socio tiene una multa activa hasta el {fechaFinMulta:dd/MM/yyyy}. No puede solicitar préstamos.";
      return View();
  }

        // Socio válido, mostrar formulario de libro
      ViewBag.SocioValido = true;
       ViewBag.NumeroSocio = numeroSocio;
    ViewBag.NombreSocio = socio.Nombre;
       ViewBag.IdSocio = socio.IdSocio;
       return View();
            }

            // Paso 2: Verificar Estado del Libro
       if (accion == "verificarLibro")
       {
                if (string.IsNullOrWhiteSpace(tituloLibro))
      {
       ViewBag.Error = "Debe ingresar el título del libro.";
       ViewBag.SocioValido = true;
         ViewBag.NumeroSocio = numeroSocio;
             ViewBag.IdSocio = idSocio;
         return View();
     }

    var libro = _context.Libros.FirstOrDefault(l => l.Titulo.ToLower() == tituloLibro.ToLower());

         if (libro == null)
     {
    ViewBag.Error = "El libro no existe en el catálogo.";
     ViewBag.SocioValido = true;
       ViewBag.NumeroSocio = numeroSocio;
        ViewBag.IdSocio = idSocio;
          return View();
         }

                if (libro.CantidadDisponible < 1)
    {
ViewBag.Error = "No hay ejemplares disponibles para préstamo en este momento.";
    ViewBag.SocioValido = true;
     ViewBag.NumeroSocio = numeroSocio;
                    ViewBag.IdSocio = idSocio;
              return View();
      }

                // Libro disponible, mostrar confirmación
   ViewBag.LibroValido = true;
    ViewBag.SocioValido = true;
       ViewBag.NumeroSocio = numeroSocio;
      ViewBag.IdSocio = idSocio;
  ViewBag.IdLibro = libro.Id;
             ViewBag.TituloLibro = libro.Titulo;
    ViewBag.Autor = libro.Autor;
              ViewBag.Disponibles = libro.CantidadDisponible;
      return View();
   }

            // Paso 3: Registrar Préstamo y Cambiar Estado
            if (accion == "registrarPrestamo" && idSocio.HasValue && idLibro.HasValue)
       {
            try
     {
        var libro = _context.Libros.Find(idLibro.Value);
             if (libro == null || libro.CantidadDisponible < 1)
           {
               ViewBag.Error = "Error: El libro ya no está disponible.";
            return View();
            }

         var fechaPrestamo = DateTime.Now;
                    var prestamo = new Prestamo
           {
        IdSocio = idSocio.Value,
            IdLibro = idLibro.Value,
   FechaPrestamo = fechaPrestamo,
         FechaCorrespondienteDevolucion = fechaPrestamo.AddDays(7),
      FechaDevolucion = null
      };

     _context.Prestamos.Add(prestamo);
       libro.CantidadDisponible--;
       _context.SaveChanges();

    ViewBag.Exito = $"? Préstamo registrado exitosamente. Fecha límite de devolución: {prestamo.FechaCorrespondienteDevolucion:dd/MM/yyyy}";
        ViewBag.PrestamoRealizado = true;
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
