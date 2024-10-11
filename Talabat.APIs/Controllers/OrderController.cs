using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order;
using Talabat.Core.Services.contract;

namespace Talabat.APIs.Controllers
{

	public class OrderController : BaseApiController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;

		public OrderController(IOrderService service,IMapper mapper)
		{
			_orderService = service;
			_mapper = mapper;
		}
		[HttpPost]
		[Authorize]
		public async Task<ActionResult<OrderDto>> CreateOrder(OrderDto orderDto)
		{
			// create order
			//[HttpPost] //Post => api/Orders

			var buyrEmail = User.FindFirstValue(ClaimTypes.Email);
			var mappedAddress = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);

			var userOrder = await _orderService.CreateOrderAsync(buyrEmail, orderDto.BasketId, orderDto.DeliveryMethodId, mappedAddress);

			if (userOrder == null) return BadRequest(new ApiResponse(400, "There is a Problem With Your Order"));

			return Ok(userOrder);
		}
		[ProducesResponseType(typeof(IReadOnlyList<OrderDto>),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
		{
			string buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var orders = await _orderService.GetOrdersForSpecificUserAsync(buyerEmail);
			if (orders is null) return NotFound(new ApiResponse( 404, "There is no orders For This User"));
			return Ok(orders);
		}
		[ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<OrderDto>> GetOrderForUser(int id)
		{
			string buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var orders = await _orderService.GetOrderByIdForSpecificUserAsync(buyerEmail,id);
			if (orders is null) return NotFound(new ApiResponse( 404, $"There is no order For This id {id}"));
			return Ok(orders);
		}

		[HttpGet("deliveryMethods")]
		[Authorize]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
		{
			var deliveryMethods = await _orderService.GetDeliveryMethodAsync();
			return Ok(deliveryMethods);
		}
	}
}
