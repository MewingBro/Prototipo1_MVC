using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Prototipo1.Data;
using Prototipo1.Migrations;
using Prototipo1.Models;
using Prototipo1.Models.ViewModels;
using Prototipo1.Repository.IRepository;

namespace Prototipo1.Repository
{
    public class FacturaSalidaProductoRepository : Repository<FacturaSalidaProducto>, IFacturaSalidaProductoRepository
    {
        private AppDBContext _db;
        public FacturaSalidaProductoRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }

        public void RemoveFromInventario(FacturaSalidaProducto facturaSalidaProducto, int? idProyecto)
        {
            if (!idProyecto.HasValue)
                throw new ArgumentNullException(nameof(idProyecto), "El IdProyecto no puede ser nulo.");

            int idProyecto1 = idProyecto.Value;

            //  Actualizar Inventario del proyecto
            var inventario = _db.Inventario
                .FirstOrDefault(i => i.IdProducto == facturaSalidaProducto.IdProducto && i.IdProyecto == idProyecto1);

            if (inventario == null)
                throw new InvalidOperationException($"El producto {facturaSalidaProducto.IdProducto} no existe en el inventario del proyecto.");

            if (inventario.Existencias < facturaSalidaProducto.CantidadDisminuida)
                throw new InvalidOperationException($"No hay suficiente stock de {facturaSalidaProducto.IdProducto} en el proyecto.");

            inventario.Existencias -= facturaSalidaProducto.CantidadDisminuida;
            _db.Inventario.Update(inventario);

            var factura = _db.Factura.FirstOrDefault(m => m.IdFactura == facturaSalidaProducto.IdFactura);
            int? idRecinto = factura.IdRecinto;

            // 🔹 2. Actualizar Existencias del producto en el recinto
            var recintoProducto = _db.RecintoProducto
                .FirstOrDefault(rp => rp.IdRecinto == idRecinto
                                   && rp.IdProducto == facturaSalidaProducto.IdProducto);

            if (recintoProducto != null)
            {
                // Verifica que no quede negativo
                if (recintoProducto.ExistenciasActuales < facturaSalidaProducto.CantidadDisminuida)
                {
                    throw new InvalidOperationException($"El recinto no tiene suficiente stock del producto {facturaSalidaProducto.IdProducto}.");
                }

                recintoProducto.ExistenciasActuales -= facturaSalidaProducto.CantidadDisminuida;
                _db.RecintoProducto.Update(recintoProducto);
            }
            else
            {
                // Si no existe el registro, lanzamos excepción
                throw new InvalidOperationException($"El producto {facturaSalidaProducto.IdProducto} no está presupuestado en el recinto.");
            }

            // 🔹 3. Guardar los cambios
            _db.SaveChanges();
        }




        public void Update(FacturaSalidaProducto facturaSalidaProducto)
        {
            var facturaSalidaProductoFromDB = _db.FacturaSalidaProducto.FirstOrDefault(u => u.IdFacturaSalidaProducto == facturaSalidaProducto.IdFacturaSalidaProducto);
            if (facturaSalidaProductoFromDB != null)
            {
                facturaSalidaProductoFromDB.IdFacturaSalidaProducto = facturaSalidaProducto.IdFacturaSalidaProducto;
                facturaSalidaProductoFromDB.IdFactura = facturaSalidaProducto.IdFactura;
                facturaSalidaProductoFromDB.IdProducto = facturaSalidaProducto.IdProducto;
                facturaSalidaProductoFromDB.CantidadDisminuida = facturaSalidaProducto.CantidadDisminuida;
                facturaSalidaProductoFromDB.EntregadoA = facturaSalidaProducto.EntregadoA;
                /*
                var inventarioFromDB = _db.Inventario
                .FirstOrDefault(i => i.IdProducto == facturaProducto.IdProducto);


                if (inventarioFromDB != null)
                {
                    // Actualizar existencias
                    inventarioFromDB.Existencias += facturaProducto.CantidadAumentada;
                }
                */

            }
        }
    }
}
