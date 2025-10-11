using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Prototipo1.Repository.IRepository;
using Prototipo1.Data;
using Microsoft.EntityFrameworkCore;

namespace Prototipo1.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IFamiliaRepository Familia { get; }

        IUnidadRepository Unidad { get; }

        IProductoRepository Producto { get; }

        IFacturaRepository Factura { get; }

        ITipoFacturaRepository TipoFactura { get; }

        IFacturaProductoRepository FacturaProducto { get; }

        IInventarioRepository Inventario { get; }

        IProyectoRepository Proyecto {  get; }

        INivelRepository Nivel { get; }

        IAposentoRepository Aposento { get; }

        IRecintoRepository Recinto { get; }

        void Save();
    }
}
