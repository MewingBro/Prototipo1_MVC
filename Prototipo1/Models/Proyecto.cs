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
    public class Proyecto
    {
        [Key]
        public int IdProyecto { get; set; }

        [Required(ErrorMessage = "Debe indicar el nombre del proyecto")]
        [Display(Name = "Nombre del proyecto")]
        public string NombreProyecto { get; set; }

        [ValidateNever]
        public string ImageURL { get; set; }

    }
}
