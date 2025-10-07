using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Prototipo1.Models
{
    public class TipoFactura
    {
        [Key]
        public int IdTipoFactura { get; set; }
        public string NombreTipoFactura { get; set; }

    }
}
