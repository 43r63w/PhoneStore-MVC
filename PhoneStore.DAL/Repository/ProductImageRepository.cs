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
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationdDBContext _dbContext;
        public ProductImageRepository(ApplicationdDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(ProductImage obj)
        {
            _dbContext.ProductImages.Update(obj);
        }
    }
}
