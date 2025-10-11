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
    public class RecintoRepository : Repository<Recinto>,IRecintoRepository
    {
        private AppDBContext _db;
        public RecintoRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Recinto Recinto)
        {
            var RecintoFromDB = _db.Recinto.FirstOrDefault(u => u.IdRecinto == Recinto.IdRecinto);
            if (RecintoFromDB != null)
            {
                RecintoFromDB.IdRecinto = Recinto.IdRecinto;
                RecintoFromDB.NombreRecinto = Recinto.NombreRecinto;
                RecintoFromDB.IdAposento= Recinto.IdAposento;
           
            
            }
        }
    }
}
