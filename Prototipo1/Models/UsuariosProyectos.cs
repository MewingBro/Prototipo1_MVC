using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Prototipo1.Data;
using Prototipo1.Models;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prototipo1.Models
{
    public class UsuariosProyectos
    {
        [Key]
        public int IdUsuariosProyectos { get; set; }


        public string IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        [ValidateNever]
        public IdentityUser Usuario { get; set; }


        public int IdProyecto { get; set; }
        [ForeignKey("IdProyecto")]
        [ValidateNever]
        public Proyecto Proyecto { get; set; }
    }
}
