using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class CambioDetalleVM
    {
        public string NombreProducto { get; set; }
        public int CantidadDisminuida { get; set; }
        public int ExistenciasActuales { get; set; }
        public double? Presupuesto { get; set; }
        public double InventarioProyecto { get; set; }
        public bool SeExcede { get; set; }
    }

}
