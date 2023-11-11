using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.DAL.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {

        Task AddAsync(T entity);
        void Add(T entity);

        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, string? IncludeProperties = null, bool tracked = false);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? IncludedProperties = null);

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? IncludedProperties = null);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

    }
}
