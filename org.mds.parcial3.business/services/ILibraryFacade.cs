using trabajoMetodologiaDSistemas.Models;

namespace org.mds.parcial3.business.services
{
    public interface ILibraryFacade
    {
        Task<(bool Success, string? Error, string? NumeroSocio)> RegisterMemberAsync(Socio socio);
        Task<(bool Success, string? Error, DateTime? DueDate)> RequestLoanAsync(string numeroSocio, string tituloLibro);
        Task<IEnumerable<Prestamo>> GetMemberLoansAsync(string numeroSocio);
        Task<(bool Success, string? Error)> ReturnBookAsync(int idPrestamo);
        Task<IEnumerable<Multa>> GetActiveFinesAsync(int idSocio);
    }
}