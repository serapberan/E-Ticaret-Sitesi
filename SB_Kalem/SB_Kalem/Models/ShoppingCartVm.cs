using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SB_Kalem.Models
{
    public class ShoppingCartVm
    {
        //2 modeli tek yapıda topladık
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public OrderHeader orderHeader { get; set; }
    }
}
