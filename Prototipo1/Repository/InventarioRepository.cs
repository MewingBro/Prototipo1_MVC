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
    public class InventarioRepository : Repository<Inventario>, IInventarioRepository
    {
        private AppDBContext _db;
        public InventarioRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Inventario inventario)
        {
            var InventarioFromDB = _db.Inventario.FirstOrDefault(u => u.IdInventario == inventario.IdInventario);
            if (InventarioFromDB != null)
            {
                InventarioFromDB.IdInventario = inventario.IdInventario;
                InventarioFromDB.IdProducto = inventario.IdProducto;
                InventarioFromDB.Existencias = inventario.Existencias;       
            
            }
        }
    }
}
