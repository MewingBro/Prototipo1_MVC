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
using Prototipo1.Repository.IRepository;

namespace Prototipo1.Repository
{
    public class RecintoProductoRepository : Repository<RecintoProducto>, IRecintoProductoRepository
    {
        private AppDBContext _db;
        public RecintoProductoRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(RecintoProducto RecintoProducto)
        {
            var RecintoProductoFromDB = _db.RecintoProducto.FirstOrDefault(u => u.IdRecintoProducto == RecintoProducto.IdRecintoProducto);
            if (RecintoProductoFromDB != null)
            {
                RecintoProductoFromDB.IdRecintoProducto = RecintoProducto.IdRecintoProducto;
                RecintoProductoFromDB.IdProducto = RecintoProducto.IdProducto;
                RecintoProductoFromDB.IdRecinto = RecintoProducto.IdRecinto;
                RecintoProductoFromDB.Presupuesto = RecintoProducto.Presupuesto;
                RecintoProductoFromDB.Desperdicio = RecintoProducto.Desperdicio;
                RecintoProductoFromDB.ExistenciasActuales =RecintoProducto.ExistenciasActuales;
         
            
            }
        }
    }
}
