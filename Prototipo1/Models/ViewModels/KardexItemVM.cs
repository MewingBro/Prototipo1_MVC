using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class KardexItemVM
    {
        public DateTime FechaFactura { get; set; }
        public int IdFactura { get; set; }
        public string TipoFactura { get; set; } // Entrada o Salida
        public int CantidadAumentada { get; set; }
        public int CantidadDisminuida { get; set; }
        public int Saldo { get; set; }
        public string Nivel { get; set; }
        public string Aposento { get; set; }
        public string Recinto { get; set; }
        public string Comentario { get; set; }
    }
}
