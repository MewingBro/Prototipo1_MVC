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
    public class UsuariosProyectosRepository : Repository<UsuariosProyectos>, IUsuariosProyectosRepository
    {
        private AppDBContext _db;
        public UsuariosProyectosRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(UsuariosProyectos usuariosProyectos)
        {
            var usuariosProyectosFromDB = _db.UsuariosProyectos.FirstOrDefault(u => u.IdUsuariosProyectos == usuariosProyectos.IdUsuariosProyectos);
            if (usuariosProyectosFromDB != null)
            {
                usuariosProyectosFromDB.IdUsuariosProyectos = usuariosProyectos.IdUsuariosProyectos;
                usuariosProyectosFromDB.IdUsuario = usuariosProyectos.IdUsuario;
                usuariosProyectosFromDB.IdProyecto = usuariosProyectos.IdProyecto;

            }
        }
    }
}
