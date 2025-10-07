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
    public class ProyectoRepository : Repository<Proyecto>,IProyectoRepository
    {
        private AppDBContext _db;
        public ProyectoRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Proyecto Proyecto)
        {
            var ProyectoFromDB = _db.Proyecto.FirstOrDefault(u => u.IdProyecto == Proyecto.IdProyecto);
            if (ProyectoFromDB != null)
            {
                ProyectoFromDB.IdProyecto = Proyecto.IdProyecto;
                ProyectoFromDB.NombreProyecto = Proyecto.NombreProyecto;
                if (Proyecto.ImageURL != null)
                {
                    ProyectoFromDB.ImageURL = Proyecto.ImageURL;
                }
            
            }
        }
    }
}
