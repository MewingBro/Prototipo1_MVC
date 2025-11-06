using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class KardexVM
    {
        public Producto Producto { get; set; }
        public IEnumerable<KardexItemVM> Movimientos { get; set; } = new List<KardexItemVM>();
        public IEnumerable<Recinto> Recintos { get; set; } = new List<Recinto>();
        public int? RecintoSeleccionadoId { get; set; }
    }

}
