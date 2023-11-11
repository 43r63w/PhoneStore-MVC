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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationdDBContext _dbContext;
        public OrderDetailRepository(ApplicationdDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(OrderDetail obj)
        {
            _dbContext.OrderDetails.Update(obj);
        }
    }
}
