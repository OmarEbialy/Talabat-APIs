using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
    public class BasketController : BaseAPIsController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository , IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            return basket is null ? new CustomerBasket(id) : basket;  
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateCustomerBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
            if (CreatedOrUpdatedBasket == null) return BadRequest(new ApiResponse(400));
            return Ok(CreatedOrUpdatedBasket);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteCustomerBasket(string id)
        {
            return await _basketRepository.DeleteBasketAsync(id);
        }
    }
}
