using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace ASC.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();

        Task<T?> GetByIdAsync(string id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
    }
}