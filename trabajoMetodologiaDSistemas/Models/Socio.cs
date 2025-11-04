using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace trabajoMetodologiaDSistemas.Models
{
    [Table("tbl_socio")]
    public class Socio
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_socio")]
        public int IdSocio { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("numero_socio")]
        public string NumeroSocio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(20, ErrorMessage = "El DNI no puede exceder los 20 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El DNI debe contener solo números")]
        [Column("dni")]
        public string Dni { get; set; } = string.Empty;

        public ICollection<Prestamo>? Prestamos { get; set; }
        public ICollection<Multa>? Multas { get; set; }

    }
}
