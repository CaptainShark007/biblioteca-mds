using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trabajoMetodologiaDSistemas.Models
{
    [Table("tbl_prestamo")]
    public class Prestamo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_prestamo")]
        public int IdPrestamo { get; set; }

        [Column("id_socio")]
        public int IdSocio { get; set; }

        [Column("id_libro")]
        public int IdLibro { get; set; }

        [Column("fecha_prestamo")]
        public DateTime FechaPrestamo { get; set; }

        [Column("fecha_devolucion")]
        public DateTime? FechaDevolucion { get; set; }

        [Column("fecha_correspondiente_devolucion")]
        public DateTime FechaCorrespondienteDevolucion { get; set; }

        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "Activo";

        [ForeignKey(nameof(IdSocio))]
        public Socio? Socio { get; set; }

        [ForeignKey(nameof(IdLibro))]
        public Libro? Libro { get; set; }

        // Métodos según diagrama de clases
        public void RegistrarPrestamo()
        {
            FechaPrestamo = DateTime.Now;
            FechaCorrespondienteDevolucion = DateTime.Now.AddDays(7);
            Estado = "Activo";
        }

        public void RegistrarDevolucion()
        {
            FechaDevolucion = DateTime.Now;
            Estado = "Devuelto";
        }

        public string ConsultarEstado()
        {
            if (FechaDevolucion != null)
                return "Devuelto";
            
            if (DateTime.Now > FechaCorrespondienteDevolucion)
                return "Atrasado";
            
            return "Activo";
        }
    }
}
