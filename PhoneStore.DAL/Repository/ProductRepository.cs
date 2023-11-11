using PhoneStore.DAL.Data;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.DAL.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationdDBContext _dbContext;
        public ProductRepository(ApplicationdDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Product> Search(string search)
        {
            return _dbContext.Products.Where(u => u.Name.Contains(search) || u.Model.Contains(search)).ToList();
        }

        public void Update(Product obj)
        {
            _dbContext.Products.Update(obj);
        }
    }
}
