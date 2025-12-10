using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Prototipo1.Models
{
    public class Factura
    {
        [Key]
        public int IdFactura { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Debe seleccionar una fecha")]
        public DateTime? Fecha { get; set; }
        public int IdTipoFactura { get; set; }
        [ForeignKey("IdTipoFactura")]
        [ValidateNever]
        public TipoFactura TipoFactura { get; set; }
        [ValidateNever]
        public string Comentario { get; set; }

        [Required(ErrorMessage = "Debe indicar la persona que recibió el producto")]
        public string RecibidoPor { get; set; }

        public int IdProyecto { get; set; }
        [ForeignKey("IdProyecto")]
        [ValidateNever]
        public Proyecto Proyecto { get; set; }


        public int? IdRecinto { get; set; }
        [ForeignKey("IdRecinto")]
        [ValidateNever]
        [AllowNull]
        public Recinto? Recinto { get; set; }


        [ValidateNever]
        public int EstadoFactura { get; set; }

    }
}
