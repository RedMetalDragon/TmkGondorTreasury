using Microsoft.AspNetCore.Mvc;
using TmkGondorTreasury.Services;
using TmkGondorTreasury.DTOs;
using Stripe;

namespace TmkGondorTreasury.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SessionStorageService _sessionStorageService;
        private readonly StripeRegistrationService _stripeRegistrationService;

        public UsersController(SessionStorageService sessionStorageService, StripeRegistrationService stripeRegistrationService)
        {
            _sessionStorageService = sessionStorageService;
            _stripeRegistrationService = stripeRegistrationService;
        }

        // POST: api/users/register-new-user/step-one
        [HttpPost("/register-new-user/step-one")]
        public async Task<ActionResult<string>> RegisterNewUser([FromBody] UserDto userDto)
        {
            // In this step the user sends the user data to be saved in the session storage
            try
            {
                await _sessionStorageService.SaveUser(userDto);
                return Created("", "Step one completed successfully!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        // POST: api/users/register-new-user/step-two
        [HttpPost("/register-new-user/step-two")]
        public async Task<ActionResult<string>> RegisterNewUserStepTwo([FromBody] PaymentIntentDto paymentIntentDto)
        {
            // In this step the user is already saved in the session storage
            // and sends the subcription plan and payment method to create the payment intent
            try
            {
                var user = await _sessionStorageService.GetUser(paymentIntentDto.UserDto?.Email ?? "");
                if (user == null)
                {
                    return BadRequest("User not found in session storage");
                }
                try
                {
                    var paymentIntent = await _stripeRegistrationService.CreatePaymentIntent(paymentIntentDto);
                    return Ok(new { clientSecret = paymentIntent.ClientSecret });
                }
                catch (StripeException)
                {
                    return BadRequest("Error in the payment flow. [$$-1]");
                }
            }
            catch (Exception)
            {
                return BadRequest("Error retrieving temporary user. [$$-2]");
            }
        }
    }
}