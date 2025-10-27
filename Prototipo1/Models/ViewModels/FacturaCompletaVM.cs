namespace Prototipo1.Models.ViewModels
{
    public class FacturaCompletaVM
    {
        public Factura Factura { get; set; }
        public IEnumerable<FacturaProducto> Detalles { get; set; }
    }
}
