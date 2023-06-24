using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Services
{
    public interface IOrderServices
    {
        Task<Order> CreateOrderAsync(string buyerEmail , string basketId , int deliveryMethodId , Address shippingAddress);
        Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail);
        Task<Order?> GetOrderByIdForUserAsync (string buyerEmail , int orderId);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodForUserAsync();
    }
}
