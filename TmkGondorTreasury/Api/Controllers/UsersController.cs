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
                var customer = await _stripeRegistrationService.CreateCustomer(userDto);
                var paymentIntent = await _stripeRegistrationService.CreatePaymentIntentForSubscription(new PaymentIntentDto
                {
                    CustomerId = customer.Id,
                    UserDto = userDto,
                    SubscriptionPlan = userDto.SubsriptionPlan switch
                    {
                        "standard" => DTOs.Enums.SubscriptionPlan.Standard,
                        "basic" => DTOs.Enums.SubscriptionPlan.Basic,
                        "premiun" => DTOs.Enums.SubscriptionPlan.Premium,
                        _ => throw new NotImplementedException(),
                    }

                });
                return Ok(new { paymentIntent.ClientSecret });
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
    }
}