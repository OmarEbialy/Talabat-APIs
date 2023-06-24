using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;
using Order = Talabat.Core.Entities.Order_Aggregate.Order;

namespace Talabat.Core.Specifications
{
    public class OrderSpecifications : BaseSpecifications<Order>
    {
        public OrderSpecifications(string email):base(O => O.BuyerEmail == email)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
            AddOrderByDecsending(O => O.OrderDate);
        }

        public OrderSpecifications(string email , int orderId) : base(O => O.BuyerEmail == email && O.Id == orderId)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
        }

    }
}
