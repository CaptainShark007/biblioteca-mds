public class Prestamo
{
    public int IdPrestamo { get; set; }
    public int IdSocio { get; set; }
    public int IdLibro { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaCorrespondienteDevolucion { get; set; }
    public DateTime? FechaDevolucion { get; set; }
}