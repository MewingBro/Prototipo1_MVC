using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class CambioVM
    {
        public Cambio Cambio { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> FacturaList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> UsuarioList { get; set; }

    }
}
