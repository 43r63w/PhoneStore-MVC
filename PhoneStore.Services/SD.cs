using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.Services
{
    public class SD
    {
        public const string Role_Admin = "Адмін";
        public const string Role_Customer = "Користувач";

        public const string ShoppingCartSessionId = "Ключ до сесії кошика";
        public const string WishlistSessionId = "Ключ до сесії списка бажаного";




        public const string OrderWaiting = "Очікується";
        public const string OrderPlacedAndPaid = "Замовлення оформленно і оплаченно";
        public const string OrderBeingShipped = "Замовлення відвантажується";
        public const string OrderCancelled = "Замовлення скасованно";
        public const string OrderDelivered = "Замовлення доставленно";



        public const string PaymentPaid = "Оплаченно";   
        public const string PaymentRefund = "Гроші повернуто";
        public const string PaymentWaiting = "Очікується оплата";

















    }
}
