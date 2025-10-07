using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Prototipo1.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties = null);
        IEnumerable<T> GetAllBYID(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T GetID(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
