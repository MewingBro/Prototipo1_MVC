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

    public class RecintoProducto
    {
        [Key]
        public int IdRecintoProducto { get; set; }

        public int IdRecinto { get; set; }
        [ForeignKey("IdRecinto")]
        [ValidateNever]
        public Recinto Recinto { get; set; }

        public int IdProducto { get; set; }
        [ForeignKey("IdProducto")]
        [ValidateNever]
        public Producto Producto { get; set; }

        public double Presupuesto { get; set; }
        public double Desperdicio { get; set; }
        public double ExistenciasActuales { get; set; }


    }
}
