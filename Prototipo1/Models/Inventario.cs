using Microsoft.AspNetCore.Mvc;
using Prototipo1.Data;
using Prototipo1.Models;
using System.ComponentModel;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Numerics;

namespace Prototipo1.Models
{
    public class Inventario
    {
        [Key]
        public int IdInventario { get; set; }

        public int IdProducto { get; set; }
        [ForeignKey("IdProducto")]
        [ValidateNever]
        public Producto Producto { get; set; }


        [Required(ErrorMessage = "Debe indicar las existencias")]
        public double Existencias { get; set; }

        public int IdProyecto { get; set; }
        [ForeignKey("IdProyecto")]
        [ValidateNever]
        public Proyecto Proyecto { get; set; }


    }
}
