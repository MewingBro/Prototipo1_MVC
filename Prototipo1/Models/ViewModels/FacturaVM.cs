using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class FacturaVM
    {
        public Factura Factura { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> TipoFacturaList { get; set; }

        public IEnumerable<SelectListItem> ProyectoList { get; set; }

    }
}
