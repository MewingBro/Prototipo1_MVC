using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prototipo1.Models
{
    public class Factura
    {
        [Key]
        public int IdFactura { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public int IdTipoFactura { get; set; }
        [ForeignKey("IdTipoFactura")]
        [ValidateNever]
        public TipoFactura TipoFactura { get; set; }
        [Required]
        public string Comentario { get; set; }

        public int IdProyecto { get; set; }
        [ForeignKey("IdProyecto")]
        [ValidateNever]
        public Proyecto Proyecto { get; set; }

        [ValidateNever]
        public int EstadoFactura { get; set; }

    }
}
