using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trabajoMetodologiaDSistemas.Models
{
    [Table("tbl_multa")]
    public class Multa
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_multa")]
        public int IdMulta { get; set; }

        [Column("id_socio")]
        public int IdSocio { get; set; }

        [Column("id_libro")]
        public int IdLibro { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("monto")]
        public decimal Monto { get; set; } = 0;

        [Column("dias_restringido")]
        public int DiasRestringido { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "Activa";

        [ForeignKey(nameof(IdSocio))]
        public Socio? Socio { get; set; }

        [ForeignKey(nameof(IdLibro))]
        public Libro? Libro { get; set; }

        // Métodos según diagrama de clases
        public void RegistrarPago()
        {
            Estado = "Pagada";
        }

        public string ConsultarEstado()
        {
            if (Estado == "Pagada")
                return "Pagada";
            
            if (Fecha.AddDays(DiasRestringido) <= DateTime.Now)
                return "Expirada";
            
            return "Activa";
        }
    }
}
