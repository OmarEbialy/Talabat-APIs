using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrdersController : BaseAPIsController
    {
        private readonly IOrderServices _orderServices;
        private readonly IMapper _mapper;

        public OrdersController(IOrderServices orderServices , IMapper mapper)
        {
            _orderServices = orderServices;
            _mapper = mapper;
        }

        [HttpPost] // .../api/Orders
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var shippingAddress = _mapper.Map<AddressDto, Address>(orderDto.shipToAddress);

            var order = await _orderServices.CreateOrderAsync(buyerEmail , orderDto.BasketId , orderDto.DeliveryMethod , shippingAddress);

            if (order == null) return BadRequest(new ApiResponse(400));

            return Ok(order);
        }

        [HttpGet] // .../api/Orders
        public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _orderServices.GetOrderForUserAsync(buyerEmail);

            return Ok(orders);
        }

        [HttpGet("{id}")] // .../api/Orders/1
        public async Task<ActionResult<Order>> GetOrderForUser(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var order = await _orderServices.GetOrderByIdForUserAsync(email , id);

            if (order == null ) return NotFound(new ApiResponse(404));

            return Ok(order);

        }

        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _orderServices.GetDeliveryMethodForUserAsync();

            return Ok(deliveryMethods);
        }
    }
}
