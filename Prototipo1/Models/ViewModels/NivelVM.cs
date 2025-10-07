using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class NivelVM
    {
        public Nivel Nivel { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ProyectoList { get; set; }
    }
}
