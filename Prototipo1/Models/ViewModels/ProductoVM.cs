using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class ProductoVM
    {
        public Producto Producto { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> FamiliaList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> UnidadList { get; set; }
    }
}
