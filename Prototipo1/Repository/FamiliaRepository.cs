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
    public class FamiliaRepository : Repository<Familia>,IFamiliaRepository
    {
        private AppDBContext _db;
        public FamiliaRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Familia familia)
        {
            _db.Familia.Update(familia);
        }
    }
}
