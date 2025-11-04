using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trabajoMetodologiaDSistemas.Models
{
    [Table("tbl_libro")]
    public class Libro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        [Column("titulo")]
        public string Titulo { get; set; }

        [Required, MaxLength(30)]
        [Column("autor")]
        public string Autor { get; set; }

        [Required]
        [Column("isbn")]
        public int ISBN { get; set; }

        [Required]
        [Column("cantidad_disponible")]
        public int CantidadDisponible { get; set; }

        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "Disponible";

        public ICollection<Prestamo>? Prestamos { get; set; }
        public ICollection<Multa>? Multas { get; set; }

        // Método según diagrama de clases
        public bool EstaDisponible()
        {
            return CantidadDisponible > 0 && Estado == "Disponible";
        }
    }
}
