using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class InventarioVM
    {
        public Inventario Inventario { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ProductoList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ProyectoList { get; set; }

    }
}
