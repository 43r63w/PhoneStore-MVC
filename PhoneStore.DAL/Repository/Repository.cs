using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhoneStore.DAL.Data;
using PhoneStore.DAL.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationdDBContext _dbContext;
        internal DbSet<T> _set;

        public Repository(ApplicationdDBContext dbContext)
        {
            _dbContext = dbContext;
            _set = _dbContext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
        }
        public void Add(T entity)
        {
            _set.Add(entity);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, string? IncludeProperties = null, bool tracked = false)
        {
            IQueryable<T> values = _set;

            values = values.Where(filter);
            if (IncludeProperties != null)
            {
                foreach (var property in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {

                    values = values.Include(property);

                }
            }
            return await values.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? IncludedProperties = null)
        {
            IQueryable<T> values = _set;
            values = values.Where(filter);

            if (IncludedProperties != null)
            {
                foreach (var property in IncludedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    values = values.Include(property);
                }
            }

            return await values.ToListAsync();

        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? IncludedProperties = null)
        {
            IQueryable<T> values = _set;
            if (filter != null)
            {
                values = values.Where(filter);
            }

            if (IncludedProperties != null)
            {
                foreach (var property in IncludedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    values = values.Include(property);
                }
            }

            return values.ToList();

        }

        public void Remove(T entity)
        {
            _set.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _set.RemoveRange(entities);
        }

       
    }
}
