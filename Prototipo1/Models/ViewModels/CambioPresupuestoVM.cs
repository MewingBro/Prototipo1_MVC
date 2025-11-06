using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class CambioPresupuestoVM
    {
        public int IdFactura { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public double Presupuesto { get; set; }
        public double CantidadDisminuida { get; set; }
        public double Sobregiro => CantidadDisminuida - Presupuesto;
        public string Nivel { get; set; }
        public string Aposento { get; set; }
        public string Recinto { get; set; }
        public string Comentario { get; set; }
    }
}

