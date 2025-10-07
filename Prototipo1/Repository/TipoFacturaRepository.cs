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
using Prototipo1.Models.ViewModels;
using Prototipo1.Repository.IRepository;

namespace Prototipo1.Repository
{
    public class TipoFacturaRepository : Repository<TipoFactura>, ITipoFacturaRepository
    {
        private AppDBContext _db;
        public TipoFacturaRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(TipoFactura tipoFactura)
        {

        }
    }
}
