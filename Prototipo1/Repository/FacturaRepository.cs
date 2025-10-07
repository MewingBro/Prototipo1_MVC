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
using Prototipo1.Repository.IRepository;

namespace Prototipo1.Repository
{
    public class FacturaRepository : Repository<Factura>, IFacturaRepository
    {
        private AppDBContext _db;
        public FacturaRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Factura factura)
        {
            var facturaFromDB = _db.Factura.FirstOrDefault(u => u.IdFactura == factura.IdFactura);
            if (facturaFromDB != null)
            {
                facturaFromDB.IdFactura = factura.IdFactura;
                facturaFromDB.Fecha = factura.Fecha;
                facturaFromDB.IdTipoFactura = factura.IdTipoFactura;
                facturaFromDB.Comentario = factura.Comentario;
                facturaFromDB.IdProyecto = factura.IdProyecto;



            }
        }
    }
}
