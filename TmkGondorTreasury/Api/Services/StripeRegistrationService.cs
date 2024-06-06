namespace TmkGondorTreasury.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Stripe;
using TmkGondorTreasury.DTOs;
using TmkGondorTreasury.DTOs.Enums;

public class StripeRegistrationService
{
    private readonly string _stripeSecretKey;
    private readonly IConfiguration _configuration;

    public StripeRegistrationService(string stripeSecretKey, IConfiguration configuration)
    {
        _stripeSecretKey = stripeSecretKey;
        _configuration = configuration;
        StripeConfiguration.ApiKey = _stripeSecretKey;
    }

    public async Task<Customer> CreateCustomer(UserDto userDto)
    {
        try
        {
            var customer = new CustomerCreateOptions
            {
                Email = userDto.Email,
                Name = userDto.FirstName + " " + userDto.LastName,
                Metadata = new Dictionary<string, string>
                {
                    {"subscriptionPlan", userDto.SubscriptionPlan ?? ""},
                    {"firstName", userDto.FirstName ?? ""},
                    {"lastName", userDto.LastName ?? ""},
                    {"email", userDto.Email ?? ""},
                    {"createdAt", DateTime.Now.ToString()}
                }
            };
            var service = new CustomerService();
            return await service.CreateAsync(customer);
        }
        catch (StripeException)
        {

            throw new Exception("Stripe issues crashed the payment flow [$$-4]");
        }
    }

    public async Task<PaymentIntent> CreatePaymentIntentForSubscription(PaymentIntentDto paymentIntentDto)
    {
        try
        {
            var subscriptionService = new SubscriptionService();
            var priceId = GetPriceId(paymentIntentDto.SubscriptionPlan) ?? throw new ArgumentNullException("Price ID not found.");
            var subscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
            {
                Customer = paymentIntentDto?.CustomerId,
                Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Price = priceId,
                }

            }
            });
            var paymentIntentId = subscription.LatestInvoice.PaymentIntentId;
            var paymentIntentService = new PaymentIntentService();
            return await paymentIntentService.GetAsync(paymentIntentId);
        }
        catch (StripeException)
        {
            throw new Exception("Stripe issues crashed the payment flow.");
        }

        catch (Exception e)
        {
            throw new Exception(e.Message);
        }



    }
    
    /// <summary>
    /// The function `GetPriceId` retrieves the price ID for a given subscription plan from
    /// configuration settings.
    /// </summary>
    /// <param name="SubscriptionPlan">SubscriptionPlan is an enum representing different subscription
    /// plans such as Basic, Standard, and Premium.</param>
    /// <returns>
    /// The `GetPriceId` method returns the price ID associated with the provided `SubscriptionPlan`.
    /// </returns>
    public string GetPriceId(SubscriptionPlan subscriptionPlan)
    {
        string? priceId = subscriptionPlan switch
        {
            SubscriptionPlan.Basic => _configuration["Stripe:Subscriptions:PriceId:Basic"],
            SubscriptionPlan.Standard => _configuration["Stripe:Subscriptions:PriceId:Standard"],
            SubscriptionPlan.Premium => _configuration["Stripe:Subscriptions:PriceId:Premium"],
            _ => throw new Exception("Invalid subscription plan.")
        } ?? throw new Exception("Price ID not found.");
        return priceId;
    }

    /// <summary>
    /// The function `GetSubscriptionPlan` takes a subscription string and returns the corresponding
    /// SubscriptionPlan enum value.
    /// </summary>
    /// <param name="subscription">The `GetSubscriptionPlan` method takes a string parameter
    /// `subscription` which represents the type of subscription plan. The method then uses a switch
    /// statement to map the input string to the corresponding `SubscriptionPlan` enum value (`Basic`,
    /// `Standard`, or `Premium`). If the input string does not match</param>
    /// <returns>
    /// The `GetSubscriptionPlan` method returns a `SubscriptionPlan` enum value based on the input
    /// `subscription` string.
    /// </returns>
    public SubscriptionPlan GetSubscriptionPlan(string subscription)
    {
        SubscriptionPlan subscriptionPlan = subscription switch
        {
            "subscription-basic" => SubscriptionPlan.Basic,
            "subscription-standard" => SubscriptionPlan.Standard,
            "subscription-premium" => SubscriptionPlan.Premium,
            _ => throw new Exception("Invalid subscription plan.")
        };
        return subscriptionPlan;
    }

    /// <summary>
    /// This function creates a subscription for a customer with a specific price, using Stripe API,
    /// and returns the subscription ID and client secret.
    /// </summary>
    /// <param name="customerId">The `customerId` parameter in the `CreateSubscription` method is the
    /// identifier of the customer for whom you want to create a subscription. This customer should
    /// already exist in your system or in the Stripe Customer database. It is used to associate the
    /// subscription with the specific customer account.</param>
    /// <param name="priceId">The `priceId` parameter in the `CreateSubscription` method represents the
    /// ID of the price that you want to associate with the subscription. This price ID is typically
    /// obtained from Stripe when you create a product and its corresponding price. It is used to
    /// specify the billing details and amount associated with the subscription</param>
    /// <returns>
    /// The method `CreateSubscription` returns a `Task` that will eventually resolve to a
    /// `SubscriptionCreateResponse` object.
    /// </returns>
    public async Task<SubscriptionCreateResponse> CreateSubscription(string customerId, string priceId)
    {
        var paymentSettings = new SubscriptionPaymentSettingsOptions
        {
            SaveDefaultPaymentMethod = "on_subscription"
        };
        var subscriptionOptions = new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Price = priceId,
                }
            },
            PaymentSettings = paymentSettings,
            PaymentBehavior = "default_incomplete",
        };
        subscriptionOptions.AddExpand("latest_invoice.payment_intent");
        var subscriptionService = new SubscriptionService();
        try
        {
            Subscription subscription = await subscriptionService.CreateAsync(subscriptionOptions);
            return new SubscriptionCreateResponse
            {
                SubscriptionId = subscription.Id,
                ClientSecret = subscription.LatestInvoice.PaymentIntent.ClientSecret
            };
        }
        catch (StripeException)
        {
            throw;
        }
    }

}
