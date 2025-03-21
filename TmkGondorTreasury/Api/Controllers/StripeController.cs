using Microsoft.AspNetCore.Mvc;
using TmkGondorTreasury.Api.Helpers;
using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StripeController : ControllerBase
    {
        [HttpGet]
        public async Task<OkObjectResult> Get()
        {
            var subscriptions = await StripeApiHelper.GetSubscriptionsTypes();
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