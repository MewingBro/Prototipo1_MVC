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
    public class Recinto
    {
        [Key]
        public int IdRecinto { get; set; }
        [Required]
        [Display(Name = "Nombre del Recinto")]
        public string NombreRecinto { get; set; }

        public int IdAposento { get; set; }
        [ForeignKey("IdAposento")]
        [ValidateNever]
        public Aposento Aposento { get; set; }

        [ValidateNever]
        public int EstadoRecinto { get; set; }

    }
}
