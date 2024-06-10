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
        [HttpPost("register-new-user/user-details")]
        public async Task<ActionResult<string>> RegisterNewUserFirstStep([FromBody] UserDto? userDto)
        {
            // In this step the user is already saved in the session storage
            // and sends the subcription plan and payment method to create the payment intent
            try
            {
                if (userDto == null)
                {
                    return BadRequest("User data is missing.");
                }
                await _sessionStorageService.SaveUser(userDto);
                var customer = await _stripeRegistrationService.CreateCustomer(userDto);
                var subscriptionPlan = _stripeRegistrationService.GetSubscriptionPlan(userDto.SubscriptionPlan ?? throw new ArgumentNullException("Subscription plan is missing."));
                var priceId = _stripeRegistrationService.GetPriceId(subscriptionPlan);
                var result = await _stripeRegistrationService.CreateSubscription(customer.Id, priceId);
                result.CustomerId = customer.Id;
                return Ok(result);
                //
                //var setupIntent = await _stripeRegistrationService.CreateCustomerAndSetupIntent(userDto);
                //return Ok(new { success = true, setupIntent.CustomerId, setupIntent.IntentClientSecret });
            }
            catch (NotImplementedException)
            {
                return BadRequest("Subscription plan invalid. [$$-3]");
            }
            catch (StripeException)
            {
                return BadRequest("Failed to create subscription. [$$-8]");

            }
            catch (Exception)
            {
                return BadRequest("Error retrieving temporary user. [$$-2]");
            }
        }

    }
}