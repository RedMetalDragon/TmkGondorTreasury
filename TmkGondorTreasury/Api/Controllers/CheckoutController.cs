using Microsoft.AspNetCore.Mvc;
using Stripe;
using TmkGondorTreasury.Api.Interfaces;
using TmkGondorTreasury.Api.Models;

namespace TmkGondorTreasury.Api.Controllers;

[ApiController]
[Route("checkout")]
public class CheckoutController : Controller
{
    private readonly ILogger<CheckoutController> _logger;
    private readonly IStripeHelper _stripeHelper;

    public CheckoutController(ILogger<CheckoutController> logger, IStripeHelper stripeHelper)
    {
        _logger = logger;
        _stripeHelper = stripeHelper;
    }

    [HttpPost("create-checkout-session-for-subscription")]
    public async Task<IActionResult> CreateCheckoutSessionForSubscription(
        [FromBody] CreateCheckoutSessionRequest createSubscriptionRequest)
    {
        try
        {
            var fullName = $"{createSubscriptionRequest.FirstName} {createSubscriptionRequest.LastName}";
            var customer = await _stripeHelper.CreateCustomer(createSubscriptionRequest.Email, fullName);
            var session = await _stripeHelper.CreateSession(customer.Id, createSubscriptionRequest.PriceId);
            var subscription = await _stripeHelper.CreateSubscription(customer.Id, createSubscriptionRequest.PriceId);
            return Ok(new CreateCheckoutSessionResponse
            {
                IntentClientSecret = subscription.LatestInvoice.PaymentIntent.ClientSecret,
                CustomerId = customer.Id,
                Currency = subscription.Currency,
                SessionId = session.Id
            });
        }
        catch (StripeException e)
        {
            _logger.Log(LogLevel.Error, e, "Stripe Exception");
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, e, "Exception");
            return BadRequest();
        }
    }
}