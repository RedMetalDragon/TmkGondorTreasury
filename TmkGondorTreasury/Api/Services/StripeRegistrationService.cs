namespace TmkGondorTreasury.Services;
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
                    {"subscriptionPlan", userDto.SubsriptionPlan ?? ""},
                    {"firstName", userDto.FirstName ?? ""},
                    {"lastName", userDto.LastName ?? ""},
                    {"email", userDto.Email ?? ""},
                    {"createdAt", DateTime.Now.ToString()}
                }
            };
            var service = new CustomerService();
            return await service.CreateAsync(customer);
        }
        catch (StripeException e)
        {
            throw new Exception(e.Message);
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
    /// The function `GetPriceId` returns the corresponding price ID based on the given subscription
    /// plan using configuration values.
    /// </summary>
    /// <param name="SubscriptionPlan">SubscriptionPlan is an enum representing different subscription
    /// plans such as Basic, Standard, and Premium.</param>
    /// <returns>
    /// The `GetPriceId` method returns a string value, which is the price ID associated with the
    /// provided `SubscriptionPlan`.
    /// </returns>
    private string GetPriceId(SubscriptionPlan subscriptionPlan)
    {
        string? priceId = subscriptionPlan switch
        {
            SubscriptionPlan.Basic => _configuration["Stripe:PriceId:Premium"],
            SubscriptionPlan.Standard => _configuration["Stripe:PriceId:Basic"],
            SubscriptionPlan.Premium => _configuration["Stripe:PriceId:Enterprise"],
            _ => throw new Exception("Invalid subscription plan.")
        } ?? throw new Exception("Price ID not found.");
        return priceId;
    }

}
