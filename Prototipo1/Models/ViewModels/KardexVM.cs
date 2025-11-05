using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class KardexVM
    {
        public Producto Producto { get; set; }
        public List<KardexItemVM> Movimientos { get; set; } = new();
    }
}
