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

            // 🔹 Busca combinando ambos campos
            var inventario = _db.Inventario
                .FirstOrDefault(i => i.IdProducto == facturaSalidaProducto.IdProducto && i.IdProyecto == idProyecto1);

            if (inventario != null)
            {
                //Si ya existe esa combinación → actualiza existencias
                inventario.Existencias += facturaSalidaProducto.CantidadDisminuida;
                _db.Inventario.Update(inventario);
            }
            else
            {
                //Si no existe → crea un nuevo registro
                inventario = new Inventario
                {
                    IdProducto = facturaSalidaProducto.IdProducto,
                    Existencias = facturaSalidaProducto.CantidadDisminuida,
                    IdProyecto = idProyecto1
                };
                _db.Inventario.Add(inventario);
            }
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
