using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Prototipo1.Models
{
    public class Unidad
    {
        [Key]
        public int IdUnidad{ get; set; }
        [Required(ErrorMessage = "El campo es requerido")]
        [MaxLength(200)]
        [DisplayName("Nombre de la Unidad de Producto")]
        public string NombreUnidad { get; set; }
    }
}
