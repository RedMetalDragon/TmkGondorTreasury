using Stripe;
using TmkGondorTreasury.Api.Helpers;
using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasuryTest.Tests;

public class StripeApiHelperTest
{
    public StripeApiHelperTest()
    {
        var envPath = AppContext.BaseDirectory + "/.env";
        Console.WriteLine($"Looking for .env at: {envPath}");
        DotNetEnv.Env.Load(envPath);
        var apiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("STRIPE_SECRET_KEY environment variable not set");
        }
        StripeConfiguration.ApiKey = apiKey;
    }
    
    [Fact]
    public async void ReturningValidSubscriptions()
    {
        var subscriptions = await StripeApiHelper.GetSubscriptionsTypes();
        Assert.NotNull(subscriptions);
        Assert.NotEmpty(subscriptions);
    }
    
    [Fact]
    public async void ReturningValidSubscriptionBillingCycle()
    {
        var subscriptions = await StripeApiHelper.GetSubscriptionsTypes();
        var subscriptionTypes = subscriptions as SubscriptionType[] ?? subscriptions.ToArray();
        Assert.Equal(3, subscriptionTypes.Count(subscription => subscription.SubscriptionBillingCycle == SubscriptionBillingCycle.Monthly));
        Assert.Equal(3, subscriptionTypes.Count(subscription => subscription.SubscriptionBillingCycle == SubscriptionBillingCycle.Yearly));
    }
}