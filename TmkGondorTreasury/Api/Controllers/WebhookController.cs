namespace TmkGondorTreasury.Controllers;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.IO;
using System.Threading.Tasks;
using TmkGondorTreasury.DTOs;
using TmkGondorTreasury.Services;


[ApiController]
[Route("api/[controller]")]
public class WebhookController(SessionStorageService sessionStorageService) : ControllerBase
{
    private const string _webhookSecret = "whsec_...";
    private readonly SessionStorageService _sessionStorageService = sessionStorageService;

    [HttpPost]
    public async Task<IActionResult> StripeWebhookUserRegistered()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], "whsec_...");
            if (stripeEvent.Type != Events.PaymentIntentSucceeded)
            {
                return BadRequest();
            }
            if ( stripeEvent.Data.Object is not PaymentIntent paymentIntent 
                || paymentIntent.Metadata == null
                || paymentIntent.Metadata["email"] == null )
                {
                    return BadRequest();
                }
            
            return Ok(await _sessionStorageService.GetUser(paymentIntent.Metadata["email"]));
            //await _sessionStorageService.RemoveUser(paymentIntent.Metadata["email"]);
        }
        catch (StripeException)
        {
            return BadRequest();
        }

    }
}
