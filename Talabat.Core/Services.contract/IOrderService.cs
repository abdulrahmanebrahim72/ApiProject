using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Services.contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethod, Address address);

        Task<IEnumerable<Order>> GetOrdersForSpecificUserAsync(string buyerEmail);

        Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId);
        Task<IReadOnlyCollection<DeliveryMethod>> GetDeliveryMethodAsync();

	}
}
