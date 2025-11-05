using Microsoft.AspNetCore.Mvc;
using Prototipo1.Data;
using Prototipo1.Models;
using System.ComponentModel;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace Prototipo1.Models
{
    [Index(nameof(CodigoProducto), IsUnique = true)]
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }
        [Required]

        [Display(Name = "Codigo del producto")]
        public string CodigoProducto { get; set; }
        [Required]
        [Display(Name = "Nombre del producto")]
        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        [Required]
        public int MediaAritmetica { get; set; }

        public int IdFamilia { get; set; }
        [ForeignKey("IdFamilia")]
        [ValidateNever]
        public Familia Familia { get; set; }

        public int IdUnidad { get; set; }
        [ForeignKey("IdUnidad")]
        [ValidateNever]
        public Unidad Unidad { get; set; }

        [ValidateNever]
        public string ImageURL { get; set; }

    }
}
