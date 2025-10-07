using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Prototipo1.Models
{
    public class Familia
    {
        [Key]
        public int IdFamilia { get; set; }
        [Required (ErrorMessage= "El campo es requerido")]
        [MaxLength(200)]
        [DisplayName("Nombre de la Familia")]
        public string NombreFamilia { get; set; }
    }
}
