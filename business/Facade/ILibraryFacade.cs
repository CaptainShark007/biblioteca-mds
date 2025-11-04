using business.Services;
using trabajoMetodologiaDSistemas.Models;

namespace business.Facade
{
    // Patron Facade - Interfaz simplificada para operaciones complejas
    // Proporciona una interfaz unificada para un conjunto de interfaces en un subsistema
    // Facilita el uso del sistema ocultando la complejidad
    public interface ILibraryFacade
    {
        // Operaciones de Socios
        Task<(bool Success, string? Error, string? NumeroSocio)> RegistrarNuevoSocioAsync(string nombre, string dni);
        Task<Socio?> ConsultarSocioAsync(string numeroSocio);

        // Operaciones de Prestamos
        Task<(bool Success, string? Error, DateTime? FechaDevolucion)> SolicitarPrestamoAsync(string numeroSocio, string tituloLibro);
        Task<(bool Success, string? Error)> DevolverLibroAsync(int idPrestamo, bool estadoFisicoBueno);
        Task<IEnumerable<Prestamo>> ConsultarPrestamosActivosAsync(string numeroSocio);

        // Operaciones de Libros
        Task<IEnumerable<Libro>> ConsultarCatalogoAsync();
        Task<IEnumerable<Libro>> BuscarLibrosPorTituloAsync(string titulo);
        Task<bool> LibroEstaDisponibleAsync(string tituloLibro);

        // Operaciones de Multas
        Task<IEnumerable<Multa>> ConsultarMultasActivasAsync(string numeroSocio);
        Task<(bool TieneMultas, DateTime? FechaFinMulta)> VerificarMultasAsync(string numeroSocio);
    }
}
