namespace trabajoMetodologiaDSistemas.Dtos
{
    public class PrestamoDto
    {
        public int IdPrestamo { get; set; }
        public string TituloLibro { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaCorrespondienteDevolucion { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}