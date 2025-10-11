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
    public class AposentoRepository : Repository<Aposento>,IAposentoRepository
    {
        private AppDBContext _db;
        public AposentoRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Aposento Aposento)
        {
            var AposentoFromDB = _db.Aposento.FirstOrDefault(u => u.IdAposento == Aposento.IdAposento);
            if (AposentoFromDB != null)
            {
                AposentoFromDB.IdAposento = Aposento.IdAposento;
                AposentoFromDB.NombreAposento = Aposento.NombreAposento;
                AposentoFromDB.IdNivel= Aposento.IdNivel;
           
            
            }
        }
    }
}
