using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    public class PaymentsController : BaseAPIsController
    {
        private readonly IPaymentServices _paymentServices;
        private readonly ILogger<PaymentsController> _logger;
        private const string whSecret = "whsec_0413890e0fb1009ec413771d7abf58fc3c09f8107a78f48d6359f612d8ee1d8b";


        public PaymentsController(IPaymentServices paymentServices , ILogger<PaymentsController> logger)
        {
            _paymentServices = paymentServices;
            _logger = logger;
        }

        [HttpPost("{basketId}")] // .../api/payments/basketId
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentServices.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(new ApiResponse(400));

            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
            Request.Headers["Stripe-Signature"], whSecret);

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
            Order order;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    order = await _paymentServices.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id , true);
                    _logger.LogInformation("العملية نجحت يا صيع" , paymentIntent.Id);
                    break;

                case Events.PaymentIntentPaymentFailed:
                    order = await _paymentServices.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id , false);
                    _logger.LogInformation("!!العملية فشلت يا غبى !!" , paymentIntent.Id);
                    break;
            }
            return Ok();
        }
    }
}
