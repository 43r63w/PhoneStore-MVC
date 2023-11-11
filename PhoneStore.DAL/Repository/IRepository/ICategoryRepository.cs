using PhoneStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.DAL.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);

        void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus = null);

        void UpdatePaymentStatus(int id, string sessionId,string paymentIntentId);
    }
}
