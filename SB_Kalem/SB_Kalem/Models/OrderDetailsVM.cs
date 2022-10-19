using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SB_Kalem.Models
{
    public class OrderDetailsVM
    {
        //2 tablaoya tek yerde ulaştık
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }

    }
}
