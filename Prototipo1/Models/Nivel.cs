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
    public class Nivel
    {
        [Key]
        public int IdNivel { get; set; }
        [Required(ErrorMessage = "Debe indicar el nombre del nivel")]
        [Display(Name = "Nombre del Nivel")]
        public string NombreNivel { get; set; }
        public int IdProyecto { get; set; }
        [ForeignKey("IdProyecto")]
        [ValidateNever]
        public Proyecto Proyecto { get; set; }

    }
}
