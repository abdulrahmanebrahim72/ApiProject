using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories;
using Talabat.Core.Repositories.contract;
using Talabat.Core.Services.contract;
using Talabat.Core.Specifications.OrderSpecification;


namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethodId, Address shippingAddress)
        {
            //1.Get Basket From Basket Repo
            var Basket = await _basketRepository.GetBasketAsync(basketId);

            //2.Get Selected Items at Basket From Product Repo
            var OrderItems = new List<OrderItem>();
            if(Basket?.Items?.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                    OrderItems.Add(orderItem);
                }
            }

            //3.Calculate SubTotal
            var subTotal = OrderItems.Sum(item => item.Price * item.Quantity);

            //4.Get Delivery Method From DeliveryMethod Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(DeliveryMethodId);

            //5.Create Order
            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, OrderItems, subTotal);

            //6.Add Order Locally
            await _unitOfWork.Repository<Order>().AddAsync(order);

            //7.Save Order To Database[ToDo]
            var result = await _unitOfWork.CompleteAsync();
            if(result <= 0) return null;


            return order;
        }

		public async Task<IReadOnlyCollection<DeliveryMethod>> GetDeliveryMethodAsync()
		{
			return await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

		}

		public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
			var spec = new OrderWithItemsAndDeliveryMethodSpecifications( buyerEmail, orderId);
			var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
			return order;
		}

        public async Task<IEnumerable<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications(buyerEmail);
			var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

			return orders;
		}
    }
}
