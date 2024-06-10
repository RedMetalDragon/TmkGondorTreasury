using Microsoft.AspNetCore.Mvc;
using Stripe;
using TmkGondorTreasury.Utils;
using System;

namespace TmkGondorTreasury.Api.Controllers.Webhooks
{
    [ApiController]
    [Route("webhook")]
    public class StripeWebhookController : ControllerBase
    {
        [HttpPost("stripe")]
        public async Task<IActionResult> HandleStripeEvent()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            // TODO: Move to config
            // TODO: Add as ENV variable in .ENV files
            const string endpointSecret = "whsec_lOMsnM7e2sd2x6C1ahBk3rsqdFi28Kjr"; 
            Event stripeEvent;
            try
            {

                stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];
                stripeEvent = EventUtility.ConstructEvent(json,
                    signatureHeader, endpointSecret);
                //TODO line below should be go to the logging logic for this service
                Console.WriteLine($"Stripe event type: {stripeEvent.Type}");
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        // TODO Save user in DB
                        //var session = stripeEvent.Data.Object as Session;
                        Console.WriteLine("Checkout session completed");
                        //FileCreator fileCreator = new FileCreator("Checkout session completed");
                        return Ok();
                    case "invoice.paid":
                        Console.WriteLine("Invoice paid completed");
                        //FileCreator file = new FileCreator("Invoice paied");
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

        [HttpGet("text")]
        public IActionResult GetText()
        {
            return Ok("Hello World");
        }
    }
}