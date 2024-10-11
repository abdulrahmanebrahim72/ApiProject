using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.contract;
using Talabat.Core.Repositories;
using Talabat.Core.Services.contract;
using Product = Talabat.Core.Entities.Product;
using Stripe;

namespace Talabat.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(
			IConfiguration configuration,
			IBasketRepository basketRepo,
			IUnitOfWork unitOfWork)
		{
			_configuration = configuration;
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
		}


		public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
		{
			StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

			var basket = await _basketRepo.GetBasketAsync(basketId);

			if (basket == null) return null;

			var shippingPrice = 0m;

			if (basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
				shippingPrice = deliveryMethod.Cost;
				basket.ShippingPrice = shippingPrice;
			}

			foreach (var item in basket.Items)
			{
				var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
				if (item.Price != product.Price)
					item.Price = product.Price;
			}

			var service = new PaymentIntentService();

			PaymentIntent intent;

			if (string.IsNullOrEmpty(basket.PaymentIntentId))
			{
				var options = new PaymentIntentCreateOptions()
				{
					Amount = (long)basket.Items.Sum(item => item.Quantity * (item.Price * 100)) + (long)shippingPrice * 100,
					Currency = "usd",
					PaymentMethodTypes = new List<string>() { "card" }
				};

				intent = await service.CreateAsync(options);
				basket.PaymentIntentId = intent.Id;
				basket.ClientSecret = intent.ClientSecret;
			}
			else
			{
				var options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)basket.Items.Sum(item => item.Quantity * (item.Price * 100)) + (long)shippingPrice * 100,
				};
				await service.UpdateAsync(basket.PaymentIntentId, options);
			}

			await _basketRepo.UpdateBasketAsync(basket);
			return basket;
		}
	}
}
