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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationdDBContext _dbContext;
        public OrderHeaderRepository(ApplicationdDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(OrderHeader obj)
        {
           _dbContext.OrderHeaders.Update(obj);
        }

     
        public void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus = null)
        {
           var orderHeaderFromDB = _dbContext.OrderHeaders.FirstOrDefault(u=>u.Id == id);   

            if(orderHeaderFromDB != null)
            {
                orderHeaderFromDB.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    orderHeaderFromDB.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdatePaymentStatus(int id, string sessionId, string paymentIntentId)
        {
            var orderHeaderFromDB = _dbContext.OrderHeaders.FirstOrDefault(u => u.Id == id);

            if(orderHeaderFromDB != null)
            {
                if (sessionId != null)
                {
                    orderHeaderFromDB.SessionId = sessionId;
                }
                if (paymentIntentId != null)
                {
                    orderHeaderFromDB.PaymentSessionId = paymentIntentId;
                }
            }
        }
    }
}
