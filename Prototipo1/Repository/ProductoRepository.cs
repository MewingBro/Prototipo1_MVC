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
    public class ProductoRepository : Repository<Producto>,IProductoRepository
    {
        private AppDBContext _db;
        public ProductoRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Producto Producto)
        {
            var ProductoFromDB = _db.Producto.FirstOrDefault(u => u.IdProducto == Producto.IdProducto);
            if (ProductoFromDB != null)
            {
                ProductoFromDB.IdProducto = Producto.IdProducto;
                ProductoFromDB.CodigoProducto = Producto.CodigoProducto;
                ProductoFromDB.NombreProducto = Producto.NombreProducto;
                ProductoFromDB.Descripcion = Producto.Descripcion;
                ProductoFromDB.IdUnidad = Producto.IdUnidad;
                ProductoFromDB.IdFamilia = Producto.IdFamilia;
                ProductoFromDB.IdUnidad = Producto.IdUnidad;
                ProductoFromDB.MediaArtimetica = Producto.MediaArtimetica;
                if (Producto.ImageURL != null)
                {
                    ProductoFromDB.ImageURL = Producto.ImageURL;
                }
            
            
            
            
            
            }
        }
    }
}
