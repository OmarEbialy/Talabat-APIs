using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Services
{
    public class PaymentService : IPaymentServices
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IBasketRepository basketRepository , IUnitOfWork unitOfWork , IConfiguration configuration)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSetting:SecretKey"];

            // Create Payment Intent
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);

                basket.ShippingCost = deliveryMethod.Cost;
                shippingPrice = deliveryMethod.Cost;
            }


            // Get Items From Basket
            if (basket?.Items?.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    // Get Product That i Buy it
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                    if (item.Price != product.Price)
                        item.Price = product.Price;
                }

            }


            // Payment Intent
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId)) // Create Payment
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * item.Quantity * 100) + (long)shippingPrice * 100,
                    Currency = "usd", // Dolar Currency
                    PaymentMethodTypes = new List<string>() { "card"}
                };

                paymentIntent = await service.CreateAsync(options); // => Create Payment Service

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else // Update Payment
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * item.Quantity * 100) + (long)shippingPrice * 100
                };

                await service.UpdateAsync(basket.PaymentIntentId, options);
            }


            // Update Basekt
            await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool isSucceeded)
        {
            var spec = new OrderWithPaymentIntentIdSpecification(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (isSucceeded)
                order.Status = OrderStatus.PaymentRecived;
            else
                order.Status= OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
            
        }
    }
}
