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
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        private readonly ApplicationdDBContext _dbContext;
        public WishlistRepository(ApplicationdDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

     
        public void Update(Wishlist obj)
        {
            _dbContext.Wishlist.Update(obj);
        }
    }
}
