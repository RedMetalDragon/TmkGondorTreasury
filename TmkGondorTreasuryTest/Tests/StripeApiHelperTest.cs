using DotNetEnv;
using Stripe;
using TmkGondorTreasury.Api.Helpers;
using TmkGondorTreasury.Api.Models;
using TmkGondorTreasury.DTOs;
using TmkGondorTreasuryTest.Mocks;

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
        var stripeApiHelper = new MockStripeHelper();
        var subscriptions = await stripeApiHelper.GetSubscriptionsTypes();
        Assert.NotNull(subscriptions);
        Assert.NotEmpty(subscriptions);
    }
    
    [Fact]
    public async void ReturningValidSubscriptionBillingCycle()
    {
        var stripeApiHelper = new MockStripeHelper();
        var subscriptions = await stripeApiHelper.GetSubscriptionsTypes();
        var subscriptionTypes = subscriptions as SubscriptionType[] ?? subscriptions.ToArray();
        Assert.Equal(3, subscriptionTypes.Count(subscription => subscription.SubscriptionBillingCycle == SubscriptionBillingCycle.Monthly));
        Assert.Equal(3, subscriptionTypes.Count(subscription => subscription.SubscriptionBillingCycle == SubscriptionBillingCycle.Yearly));
    }
}