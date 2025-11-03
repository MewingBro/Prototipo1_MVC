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
    public class CambioRepository : Repository<Cambio>,ICambioRepository
    {
        private AppDBContext _db;
        public CambioRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Cambio Cambio)
        {
            var CambioFromDB = _db.Cambio.FirstOrDefault(u => u.IdCambio == Cambio.IdCambio);
            if (CambioFromDB != null)
            {
                CambioFromDB.IdCambio = Cambio.IdCambio;
                CambioFromDB.IdFactura = Cambio.IdFactura;
                CambioFromDB.Estado = Cambio.Estado;
                CambioFromDB.IdUsuario = Cambio.IdUsuario;

                }         
            
            }
        }
    }

