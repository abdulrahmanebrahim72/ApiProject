using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;
using Talabat.Core.Specifications;

namespace Talabat.Core.Specifications.OrderSpecification
{
    public class OrderWithItemsAndDeliveryMethodSpecifications : BaseSpecifications<Order>
    {
        public OrderWithItemsAndDeliveryMethodSpecifications()
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.ShippingAddress);
            AddOrderByDescending(O => O.OrderDate);
        }
        public OrderWithItemsAndDeliveryMethodSpecifications(string buyerEmail)
            : base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryMethod);

            AddOrderByDescending(O => O.OrderDate);
        }
        public OrderWithItemsAndDeliveryMethodSpecifications( string buyerEmail, int orderId)
            : base(O => O.BuyerEmail == buyerEmail && O.Id == orderId)
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryMethod);
        }
    }
}
