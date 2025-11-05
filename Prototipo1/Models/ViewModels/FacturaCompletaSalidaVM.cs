namespace Prototipo1.Models.ViewModels
{
    public class FacturaCompletaSalidaVM
    {
        public Factura Factura { get; set; }
        public IEnumerable<FacturaSalidaProducto> Detalles { get; set; }
    }
}
