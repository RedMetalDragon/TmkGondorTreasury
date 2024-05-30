using Microsoft.AspNetCore.Mvc;
using TmkGondorTreasury.Services;
using TmkGondorTreasury.DTOs;

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
        [HttpPost("register-new-user/step-one")]
        public async Task<ActionResult<string>> RegisterNewUserFirstStep([FromBody] UserDto userDto)
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
        [HttpPost("register-new-user/step-two")]
        public async Task<ActionResult<string>> RegisterUserSecondStep([FromBody] UserDto? userDto)
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
                var setupIntent = await _stripeRegistrationService.CreateCustomerAndSetupIntent(userDto);
                return Ok(new { setupIntent.CustomerId, setupIntent.IntentClientSecret });
            }
            catch (NotImplementedException)
            {
                return BadRequest("Subscription plan invalid. [$$-3]");
            }
            catch (Exception)
            {
                return BadRequest("Error retrieving temporary user. [$$-2]");
            }
        }

        // POST: api/users/register-new-user/attach-payment-method
        [HttpPost("register-new-user/attach-payment-method")]
        public async Task<IActionResult> AttachPaymentMethod([FromBody] PaymentIntentDto paymentIntentDto)
        {
            try
            {
                var subscription = await _stripeRegistrationService.AttachPaymentMethodAndCreateSubscription
                    (
                        paymentIntentDto.CustomerId ?? throw new ArgumentNullException("Customer ID is missing."),
                        paymentIntentDto.PaymentMethodId ?? throw new ArgumentNullException("Payment Method ID is missing."),
                        paymentIntentDto.SubscriptionPlan
                    );

                return Ok(new { succss = true, subscriptionId = subscription.Id });

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return BadRequest("Error attaching payment method. Unknown error [$$-7]");
            }

        }
    }
}