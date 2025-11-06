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
    public class CambioDetalleRepository : Repository<CambioDetalle>,ICambioDetalleRepository
    {
        private AppDBContext _db;
        public CambioDetalleRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(CambioDetalle CambioDetalle)
        {
            var CambioDetalleFromDB = _db.CambioDetalle.FirstOrDefault(u => u.IdCambioDetalle == CambioDetalle.IdCambioDetalle);
            if (CambioDetalleFromDB != null)
            {
                CambioDetalleFromDB.IdCambioDetalle = CambioDetalle.IdCambioDetalle;
                CambioDetalleFromDB.IdCambio = CambioDetalle.IdCambio;
                CambioDetalleFromDB.Presupuesto = CambioDetalle.Presupuesto;
                CambioDetalleFromDB.CantidadDisminuida= CambioDetalle.CantidadDisminuida;
                CambioDetalleFromDB.ExistenciasActuales = CambioDetalle.ExistenciasActuales;

            }         
            
            }
        }
    }

