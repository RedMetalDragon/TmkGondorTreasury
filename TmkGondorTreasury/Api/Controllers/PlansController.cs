using Microsoft.AspNetCore.Mvc;
using TmkGondorTreasury.Api.Helpers;
using TmkGondorTreasury.Api.Interfaces;
using TmkGondorTreasury.Api.Models;
using TmkGondorTreasury.Api.Services;
using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly IGondorConfigurationService _configuration;
        private readonly IStripeHelper _stripeHelper;

        public PlansController(IGondorConfigurationService configuration, IStripeHelper stripeHelper)
        {
            _configuration = configuration;
            _stripeHelper = stripeHelper;
        }

        [HttpGet]
        public async Task<OkObjectResult> Get()
        {
            var subscriptions = await _stripeHelper.GetSubscriptionsTypes();
            var subscriptionTypes = subscriptions as SubscriptionType[] ?? subscriptions.ToArray();
            var plans = new AvailablePlans
            {
                MonthlyPlans = subscriptionTypes.Where(subscription =>
                    subscription.SubscriptionBillingCycle == SubscriptionBillingCycle.Monthly).ToList(),
                YearlyPlans = subscriptionTypes.Where(subscription =>
                    subscription.SubscriptionBillingCycle == SubscriptionBillingCycle.Yearly).ToList()
            };
            return Ok(plans);
        }
    }
}