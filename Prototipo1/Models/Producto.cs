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
        [Required(ErrorMessage = "Debe indicar el código del producto")]

        [Display(Name = "Codigo del producto")]
        public string CodigoProducto { get; set; }
        [Required(ErrorMessage = "Debe indicar el nombre del producto")]
        [Display(Name = "Nombre del producto")]
        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Debe indicar la cantidad de días deseados para el posterior cálculo de esta medida")]
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
