using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;

namespace TmkGondorTreasury.Api.Controllers.Webhooks
{
    [ApiController]
    [Route("webhooks/tmk-gondor-treasury/stripe")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IConfiguration? _configuration;
        [HttpPost]
        public async Task<IActionResult> HandleStripeEvent()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Event stripeEvent;
            try
            {

                var webHookSecret = _configuration?["Stripe:WebhookSecret"];
                stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], webHookSecret);
                //TODO line below should be go to the logging logic for this service
                Console.WriteLine($"Stripe event type: {stripeEvent.Type}");
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        // TODO Save user in DB
                        //var session = stripeEvent.Data.Object as Session;
                        Console.WriteLine("Checkout session completed");
                        return Ok();
                    case "invoice.paid":
                        Console.WriteLine("Invoice paid");
                        // TODO Update user subscription
                        //var invoice = stripeEvent.Data.Object as Invoice;
                        return Ok();
                    default:
                        Console.WriteLine("Unhandled event type: " + stripeEvent.Type);
                        return Ok();
                }
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}