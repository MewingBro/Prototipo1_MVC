using Microsoft.AspNetCore.Mvc;
using Prototipo1.Data;
using Prototipo1.Models;
using System.ComponentModel;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Prototipo1.Models
{
    public class FacturaSalidaProducto
    {
        [Key]
        public int IdFacturaSalidaProducto { get; set; }

        public int IdFactura { get; set; }
        [ForeignKey("IdFactura")]
        [ValidateNever]
        public Factura Factura { get; set; }

        public int IdProducto { get; set; }
        [ForeignKey("IdProducto")]
        [ValidateNever]
        public Producto Producto { get; set; }


        [Required]
        public int CantidadDisminuida { get; set; }

        [Required]
        public string EntregadoA { get; set; }
    }
}
