using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.Order;

namespace Talabat.APIs.DTOs
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public AddressDto ShippingAddress { get; set; }

	}
}
