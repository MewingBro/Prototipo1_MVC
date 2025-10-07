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
    public class UnidadRepository : Repository<Unidad>,IUnidadRepository
    {
        private AppDBContext _db;
        public UnidadRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Unidad unidad)
        {
            _db.Unidad.Update(unidad);
        }
    }
}
