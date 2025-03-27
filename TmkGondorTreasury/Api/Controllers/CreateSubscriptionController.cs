using Microsoft.AspNetCore.Mvc;
using Stripe;
using TmkGondorTreasury.Api.Interfaces;
using TmkGondorTreasury.Api.Models;
using TmkGondorTreasury.Api.Services;
using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Api.Controllers;

[ApiController]
[Route("subscriptions")]
public class CreateSubscriptionController : Controller
{
    private readonly IGondorConfigurationService _configuration;
    private readonly ILogger<CreateSubscriptionController> _logger;
    private readonly IStripeHelper _stripeHelper;

    public CreateSubscriptionController(IGondorConfigurationService configuration,
        ILogger<CreateSubscriptionController> logger,
        IStripeHelper stripeHelper)
    {
        _configuration = configuration;
        _logger = logger;
        _stripeHelper = stripeHelper;
    }

    [HttpPost("create-subscription")]
    public async Task<IActionResult> CreateFixRateSubscription([FromBody] CreateSubscriptionRequest sessionRequest)
    {
        try
        {
            var customer = await _stripeHelper.CreateCustomer(sessionRequest.Email);
            var subscription = await _stripeHelper.CreateSubscription(customer.Id, sessionRequest.PriceId);
            return Ok(new SubscriptionCreateResponse
            {
                SubscriptionId = subscription.Id,
                ClientSecret = subscription.LatestInvoice.PaymentIntent.ClientSecret,
                CustomerId = customer.Id
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