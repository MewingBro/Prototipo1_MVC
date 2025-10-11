using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class RecintoProductoVM
    {
        public RecintoProducto RecintoProducto { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RecintoList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ProductoList { get; set; }
    }
}
