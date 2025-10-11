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
    public class Aposento
    {
        [Key]
        public int IdAposento { get; set; }
        [Required]
        [Display(Name = "Nombre del Aposento")]
        public string NombreAposento { get; set; }

        public int IdNivel { get; set; }
        [ForeignKey("IdNivel")]
        [ValidateNever]
        public Nivel Nivel { get; set; }


    }
}
