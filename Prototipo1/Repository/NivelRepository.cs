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
    public class NivelRepository : Repository<Nivel>,INivelRepository
    {
        private AppDBContext _db;
        public NivelRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Nivel Nivel)
        {
            var NivelFromDB = _db.Nivel.FirstOrDefault(u => u.IdNivel == Nivel.IdNivel);
            if (NivelFromDB != null)
            {
                NivelFromDB.IdNivel = Nivel.IdNivel;
                NivelFromDB.NombreNivel = Nivel.NombreNivel;
                NivelFromDB.IdProyecto= Nivel.IdProyecto;
           
            
            }
        }
    }
}
