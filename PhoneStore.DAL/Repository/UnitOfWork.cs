using PhoneStore.DAL.Data;
using PhoneStore.DAL.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationdDBContext _dbContext;

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        public IProductImageRepository ProductImage { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IWishlistRepository Wishlist { get; private set; }

        public IApplicationRepository ApplicationUser { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        public IOrderDetailRepository OrderDetail { get; private set; }

        public UnitOfWork(ApplicationdDBContext dbContext)
        {
            _dbContext = dbContext;
            Category = new CategoryRepository(_dbContext);
            Product = new ProductRepository(_dbContext);
            ProductImage = new ProductImageRepository(_dbContext);  
            ShoppingCart = new ShoppingCartRepository(_dbContext);  
            Wishlist = new WishlistRepository(_dbContext);
            ApplicationUser = new ApplicationUserRepository(_dbContext);    
            OrderHeader = new OrderHeaderRepository(_dbContext);    
            OrderDetail = new OrderDetailRepository(_dbContext);
        }


        public void Save()
        {
           _dbContext.SaveChanges();
        }
    }
}
