using business.Services;
using business.Repositories;
using business.UnitOfWork;
using trabajoMetodologiaDSistemas.Models;

namespace business.Facade
{
    // Patron Facade - Implementacion de fachada para simplificar operaciones
    // NOTA: Esta es una implementacion de EJEMPLO, no funcional
    // La fachada coordina multiples servicios y proporciona una API simple
    public class LibraryFacade : ILibraryFacade
    {
        private readonly ISocioService _socioService;
        private readonly IUnitOfWork _unitOfWork;

        // Constructor - Inyeccion de dependencias
        public LibraryFacade(ISocioService socioService, IUnitOfWork unitOfWork)
        {
            _socioService = socioService;
            _unitOfWork = unitOfWork;
        }

        // Operaciones de Socios - Simplifica el proceso de registro
        public async Task<(bool Success, string? Error, string? NumeroSocio)> RegistrarNuevoSocioAsync(string nombre, string dni)
        {
            // Crea el objeto Socio y delega al servicio
            var socio = new Socio
            {
                Nombre = nombre,
                Dni = dni
            };

            return await _socioService.RegistrarSocioAsync(socio);
        }

        public async Task<Socio?> ConsultarSocioAsync(string numeroSocio)
        {
            // Delega directamente al servicio
            return await _socioService.ObtenerPorNumeroSocioAsync(numeroSocio);
        }

        // Operaciones de Prestamos - Coordina validaciones y registro
        public async Task<(bool Success, string? Error, DateTime? FechaDevolucion)> SolicitarPrestamoAsync(string numeroSocio, string tituloLibro)
        {
            // Paso 1: Verificar si el socio puede solicitar prestamos
            var (canRequest, reason) = await _socioService.PuedeSolicitarPrestamoAsync(numeroSocio);
            if (!canRequest)
                return (false, reason, null);

            // Paso 2: Buscar el libro
            var libros = await _unitOfWork.Libros.GetAllAsync();
            var libro = libros.FirstOrDefault(l => l.Titulo.Equals(tituloLibro, StringComparison.OrdinalIgnoreCase));
            
            if (libro == null)
                return (false, "Libro no encontrado", null);

            if (libro.CantidadDisponible <= 0)
                return (false, "No hay ejemplares disponibles", null);

            try
            {
                // Paso 3: Iniciar transaccion
                await _unitOfWork.BeginTransactionAsync();

                // Paso 4: Crear el prestamo
                var socio = await _socioService.ObtenerPorNumeroSocioAsync(numeroSocio);
                var fechaDevolucion = DateTime.Now.AddDays(7);
                
                var prestamo = new Prestamo
                {
                    IdSocio = socio!.IdSocio,
                    IdLibro = libro.Id,
                    FechaPrestamo = DateTime.Now,
                    FechaCorrespondienteDevolucion = fechaDevolucion
                };

                await _unitOfWork.Prestamos.AddAsync(prestamo);

                // Paso 5: Actualizar disponibilidad del libro
                libro.CantidadDisponible--;
                await _unitOfWork.Libros.UpdateAsync(libro);

                // Paso 6: Confirmar transaccion
                await _unitOfWork.CommitAsync();

                return (true, null, fechaDevolucion);
            }
            catch (Exception ex)
            {
                // Revertir en caso de error
                await _unitOfWork.RollbackAsync();
                return (false, $"Error al procesar prestamo: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string? Error)> DevolverLibroAsync(int idPrestamo, bool estadoFisicoBueno)
        {
            try
            {
                // Paso 1: Obtener el prestamo
                var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(idPrestamo);
                if (prestamo == null)
                    return (false, "Prestamo no encontrado");

                if (prestamo.FechaDevolucion != null)
                    return (false, "El libro ya fue devuelto");

                await _unitOfWork.BeginTransactionAsync();

                // Paso 2: Registrar devolucion
                prestamo.FechaDevolucion = DateTime.Now;
                await _unitOfWork.Prestamos.UpdateAsync(prestamo);

                // Paso 3: Verificar si hay retraso
                if (prestamo.FechaDevolucion > prestamo.FechaCorrespondienteDevolucion)
                {
                    var diasRetraso = (prestamo.FechaDevolucion.Value.Date - prestamo.FechaCorrespondienteDevolucion.Date).Days;
                    
                    var multa = new Multa
                    {
                        IdSocio = prestamo.IdSocio,
                        IdLibro = prestamo.IdLibro,
                        Fecha = DateTime.Now,
                        DiasRestringido = diasRetraso,
                        Descripcion = $"Devolucion con {diasRetraso} dias de retraso"
                    };

                    await _unitOfWork.Multas.AddAsync(multa);
                }

                // Paso 4: Verificar estado fisico del libro
                if (!estadoFisicoBueno)
                {
                    var multa = new Multa
                    {
                        IdSocio = prestamo.IdSocio,
                        IdLibro = prestamo.IdLibro,
                        Fecha = DateTime.Now,
                        DiasRestringido = 3,
                        Descripcion = "Libro devuelto en mal estado"
                    };

                    await _unitOfWork.Multas.AddAsync(multa);
                }

                // Paso 5: Actualizar disponibilidad del libro
                var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.IdLibro);
                if (libro != null)
                {
                    libro.CantidadDisponible++;
                    await _unitOfWork.Libros.UpdateAsync(libro);
                }

                await _unitOfWork.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return (false, $"Error al procesar devolucion: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Prestamo>> ConsultarPrestamosActivosAsync(string numeroSocio)
        {
            return await _socioService.ObtenerPrestamosAsync(numeroSocio);
        }

        // Operaciones de Libros
        public async Task<IEnumerable<Libro>> ConsultarCatalogoAsync()
        {
            return await _unitOfWork.Libros.GetAllAsync();
        }

        public async Task<IEnumerable<Libro>> BuscarLibrosPorTituloAsync(string titulo)
        {
            var libros = await _unitOfWork.Libros.GetAllAsync();
            return libros.Where(l => l.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> LibroEstaDisponibleAsync(string tituloLibro)
        {
            var libros = await _unitOfWork.Libros.GetAllAsync();
            var libro = libros.FirstOrDefault(l => l.Titulo.Equals(tituloLibro, StringComparison.OrdinalIgnoreCase));
            
            return libro != null && libro.CantidadDisponible > 0;
        }

        // Operaciones de Multas
        public async Task<IEnumerable<Multa>> ConsultarMultasActivasAsync(string numeroSocio)
        {
            var multas = await _socioService.ObtenerMultasAsync(numeroSocio);
            return multas.Where(m => m.Fecha.AddDays(m.DiasRestringido) > DateTime.Now);
        }

        public async Task<(bool TieneMultas, DateTime? FechaFinMulta)> VerificarMultasAsync(string numeroSocio)
        {
            var socio = await _socioService.ObtenerPorNumeroSocioAsync(numeroSocio);
            if (socio == null)
                return (false, null);

            var tieneMultas = await _socioService.TieneMultasActivasAsync(socio.IdSocio);
            if (!tieneMultas)
                return (false, null);

            var multas = await ConsultarMultasActivasAsync(numeroSocio);
            var multaMasReciente = multas.OrderByDescending(m => m.Fecha.AddDays(m.DiasRestringido)).FirstOrDefault();
            
            return (true, multaMasReciente?.Fecha.AddDays(multaMasReciente.DiasRestringido));
        }
    }
}
