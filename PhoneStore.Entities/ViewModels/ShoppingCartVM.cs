using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.Entities.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ItemsInCart { get; set; }

        public int Count { get; set; }

        [NotMapped]
        public double OrderTotal { get; set; }

        public OrderHeader OrderHeader { get; set; }

    }
}
