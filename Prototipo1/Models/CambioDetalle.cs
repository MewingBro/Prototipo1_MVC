using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Prototipo1.Data;
using Prototipo1.Models;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prototipo1.Models { 

    public class CambioDetalle
    {
        [Key]
        public int IdCambioDetalle { get; set; }

        public int IdCambio { get; set; }
        [ForeignKey("IdCambio")]
        [ValidateNever]
        public Cambio Cambio { get; set; }

        public int IdProducto { get; set; }
        [ForeignKey("IdProducto")]
        [ValidateNever]
        public Producto Producto { get; set; }

        public double? Presupuesto { get; set; }

        public double ExistenciasActuales { get; set; }

        public double CantidadDisminuida { get; set; }
    }
}
