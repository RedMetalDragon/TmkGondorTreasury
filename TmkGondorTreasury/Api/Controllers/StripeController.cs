using Microsoft.AspNetCore.Mvc;
using TmkGondorTreasury.Api.Helpers;
using TmkGondorTreasury.Api.Models;
using TmkGondorTreasury.Api.Services;
using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly IGondorConfigurationService _configuration;

        public StripeController(IGondorConfigurationService configuration)
        {
            _configuration = configuration;
        }
        
        [HttpGet]
        public async Task<OkObjectResult> Get()
        {
            var stripeSecretKey = _configuration.GetConfigurationValue("Stripe:APIKEY");
            var stripeApiHelper = new StripeApiHelper();
            var subscriptions = await stripeApiHelper.GetSubscriptionsTypes(stripeSecretKey);
            var subscriptionTypes = subscriptions as SubscriptionType[] ?? subscriptions.ToArray();
            var plans = new PlansAvailables
            {
                MonthlyPlans = subscriptionTypes.Where(subscription => subscription.SubscriptionBillingCycle == SubscriptionBillingCycle.Monthly).ToList(),
                YearlyPlans = subscriptionTypes.Where(subscription => subscription.SubscriptionBillingCycle == SubscriptionBillingCycle.Yearly).ToList()
            };
            return Ok(plans);
        }
    }
}