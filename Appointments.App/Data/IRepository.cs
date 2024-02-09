using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.App.Data
{
    public interface IRepository<T> where T : class, new()
    {
        Task<List<T>> Get();
        Task<List<T>> GetAllWithChildren();
        Task<T> Get(int id);
        Task<List<T>> Get<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null);
        Task<T> Get(Expression<Func<T, bool>> predicate);
        AsyncTableQuery<T> AsQueryable();
        Task<int> Insert(T entity);
        Task Update(T entity);
        Task UpdateWithChildrenAsync(T entity);
        Task<int> Delete(T entity);
    }
}
