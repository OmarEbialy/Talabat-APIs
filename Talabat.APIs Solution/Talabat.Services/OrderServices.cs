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

namespace Talabat.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentServices _paymentServices;

        public OrderServices(
            IBasketRepository basketRepo ,
            IUnitOfWork unitOfWork,
            IPaymentServices paymentServices)
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentServices = paymentServices;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            // 1. Get Basket from Basekt Repo

            var basket = await _basketRepo.GetBasketAsync(basketId);

            // 2. Get Selected Items at Basket From Products Repo

            var orderItems = new List<OrderItem>();

            if (basket?.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var productsRepo = _unitOfWork.Repository<Product>();

                    if (productsRepo != null) 
                    {
                        var product = await productsRepo.GetByIdAsync(item.Id);

                        if (product != null)
                        {
                            var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                            var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                            orderItems.Add(orderItem);
                        }
                    }
                }
            }

            // 3. Calculate SubTotal

            var subTotal = orderItems.Sum(item => item.Price *  item.Quantity);

            // 4. Get DeliveryMethod From DeliveryMethods Repo

            DeliveryMethod deliveryMethod = new DeliveryMethod();

            var deliveryMethodRepo = _unitOfWork.Repository<DeliveryMethod>();

            if (deliveryMethodRepo != null)
                deliveryMethod = await deliveryMethodRepo.GetByIdAsync(deliveryMethodId);

            // 5. Create Order

            // Check if Order is Existed Before or Not
            var spec = new OrderWithPaymentIntentIdSpecification(basket.PaymentIntentId);

            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);

                await _paymentServices.CreateOrUpdatePaymentIntent(basket.Id);
            }

            var order = new Order(buyerEmail, shippingAddress , deliveryMethod , orderItems , subTotal , basket.PaymentIntentId);

            var orderRepo = _unitOfWork.Repository<Order>();
            if(orderRepo != null)
            {
                await orderRepo.Add(order);


                // 6. Save to Database [ToDo]

                var result = await _unitOfWork.Complete();
                if (result > 0)
                    return order;
            }


            return null;
        }

        public async Task<Order?> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail , orderId);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (order == null) return null;

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);

            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodForUserAsync()
        {
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

            return deliveryMethods;
        }

    }
}
