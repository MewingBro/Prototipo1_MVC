using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class FacturaProductoVM
    {
        public FacturaProducto FacturaProducto { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ProductoList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> FacturaList { get; set; }

    }
}
