using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Prototipo1.Repository.IRepository;
using Prototipo1.Data;
using Microsoft.EntityFrameworkCore;
using Prototipo1.Models;

namespace Prototipo1.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDBContext _db;
        public IFamiliaRepository Familia { get; private set; }
        public IUnidadRepository Unidad { get; private set; }
        public IProductoRepository Producto { get; private set; }

        public IFacturaRepository Factura { get; private set; }
        public ITipoFacturaRepository TipoFactura { get; private set; }

        public IFacturaProductoRepository FacturaProducto { get; private set; }

        public IInventarioRepository Inventario{ get; private set; }

        public IProyectoRepository Proyecto { get; private set; }

        public INivelRepository Nivel { get; private set; }

        public IAposentoRepository Aposento { get; private set; }

        public IRecintoRepository Recinto { get; private set; }

        public IRecintoProductoRepository RecintoProducto { get; private set; }
        
        public IUsuariosProyectosRepository UsuariosProyectos { get; private set; }

        public IFacturaSalidaProductoRepository FacturaSalidaProducto { get; private set; }

        public ICambioRepository Cambio { get; private set; }

        public UnitOfWork(AppDBContext db)
        {
            _db = db;
            Familia = new FamiliaRepository(db);
            Unidad = new UnidadRepository(db);
            Producto = new ProductoRepository(db);
            Factura = new FacturaRepository(db);
            TipoFactura = new TipoFacturaRepository(db);
            FacturaProducto = new FacturaProductoRepository(db);
            Inventario = new InventarioRepository(db);
            Proyecto = new ProyectoRepository(db);
            Nivel = new NivelRepository(db);
            Aposento = new AposentoRepository(db);
            Recinto = new RecintoRepository(db);
            RecintoProducto =  new RecintoProductoRepository(db);
            UsuariosProyectos = new UsuariosProyectosRepository(db);
            FacturaSalidaProducto = new FacturaSalidaProductoRepository(db);
            Cambio = new CambioRepository(db);
        }

        


        public void Save()
        { 
            _db.SaveChanges();
        }

    }
}
