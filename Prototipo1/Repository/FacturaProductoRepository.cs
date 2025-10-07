using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Prototipo1.Data;
using Prototipo1.Models;
using Prototipo1.Models.ViewModels;
using Prototipo1.Repository.IRepository;

namespace Prototipo1.Repository
{
    public class FacturaProductoRepository : Repository<FacturaProducto>, IFacturaProductoRepository
    {
        private AppDBContext _db;
        public FacturaProductoRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }

        public void AddWithInventario(FacturaProducto facturaProducto)
        {

            var inventario = _db.Inventario.FirstOrDefault(i => i.IdProducto == facturaProducto.IdProducto);

            if (inventario != null)
            {
                // Ya existe → sumamos existencias
                inventario.Existencias += facturaProducto.CantidadAumentada;
                _db.Inventario.Update(inventario);
            }
            else
            {
                // No existe → creamos un nuevo registro de inventario
                inventario = new Inventario
                {
                    IdProducto = facturaProducto.IdProducto,
                    Existencias = facturaProducto.CantidadAumentada
                };
                _db.Inventario.Add(inventario);
            }

        }
        

        public void Update(FacturaProducto facturaProducto)
        {
            var facturaProductoFromDB = _db.FacturaProducto.FirstOrDefault(u => u.IdFacturaProducto == facturaProducto.IdFacturaProducto);
            if (facturaProductoFromDB != null)
            {
                facturaProductoFromDB.IdFacturaProducto = facturaProducto.IdFacturaProducto;
                facturaProductoFromDB.IdFactura = facturaProducto.IdFactura;
                facturaProductoFromDB.IdProducto = facturaProducto.IdProducto;
                facturaProductoFromDB.CantidadAumentada = facturaProducto.CantidadAumentada;
                facturaProductoFromDB.EntregadoA = facturaProducto.EntregadoA;

                var inventarioFromDB = _db.Inventario
                .FirstOrDefault(i => i.IdProducto == facturaProducto.IdProducto);

                if (inventarioFromDB != null)
                {
                    // Actualizar existencias
                    inventarioFromDB.Existencias += facturaProducto.CantidadAumentada;
                }

            }
        }
    }
}
