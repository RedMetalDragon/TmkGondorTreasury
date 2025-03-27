using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using TmkGondorTreasury.Api.Interfaces;
using TmkGondorTreasury.Api.Models;

namespace TmkGondorTreasury.Api.Controllers;

[ApiController]
[Route("checkout")]
public class CheckoutController : Controller
{
    private readonly ILogger<CreateSubscriptionController> _logger;
    private readonly IStripeHelper _stripeHelper;

    public CheckoutController(ILogger<CreateSubscriptionController> logger, IStripeHelper stripeHelper)
    {
        _logger = logger;
        _stripeHelper = stripeHelper;
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession(
        [FromBody] CreateCheckoutSessionRequest createCheckoutSessionRequest)
    {
        try
        {
            var customer = await _stripeHelper.CreateCustomer(createCheckoutSessionRequest.Email);
            var checkoutSession = await _stripeHelper.CreateSession(customer.Id, createCheckoutSessionRequest.PriceId);
            var paymentIntent = await _stripeHelper.CreatePaymentIntent(customer, checkoutSession);
            return Ok(new CreateCheckoutSessionResponse
            {
                SessionId = checkoutSession.Id,
                CustomerId = customer.Id,
                Currency = paymentIntent.Currency,
                IntentClientSecret = paymentIntent.ClientSecret,
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

    [HttpPost("create-checkout-session-for-subscription")]
    public async Task<IActionResult> CreateCheckoutSessionForSubscription(
        [FromBody] CreateCheckoutSessionRequest createSubscriptionRequest)
    {
        try
        {
            var customer = await _stripeHelper.CreateCustomer(createSubscriptionRequest.Email);
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