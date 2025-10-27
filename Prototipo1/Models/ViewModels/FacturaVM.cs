using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class FacturaVM
    {
        public Factura Factura { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> TipoFacturaList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ProyectoList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RecintoList { get; set; }

    }
}
