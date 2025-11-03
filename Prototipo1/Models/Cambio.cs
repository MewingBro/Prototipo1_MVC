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

    public class Cambio
    {
        [Key]
        public int IdCambio { get; set; }

        [Required]
        public string Estado { get; set; }

        public int IdFactura { get; set; }
        [ForeignKey("IdFactura")]
        [ValidateNever]
        public Factura Factura { get; set; }

        public string IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        [ValidateNever]
        public IdentityUser Usuario { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;


    }
}
