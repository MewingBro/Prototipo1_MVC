using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Prototipo1.Models;

namespace Prototipo1.Repository.IRepository
{
    public interface ITipoFacturaRepository : IRepository<TipoFactura>
    {
        void Update(TipoFactura tipoFactura);
    }
}
