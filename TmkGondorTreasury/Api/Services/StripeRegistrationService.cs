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

    public async Task<PaymentIntent> CreatePaymentIntent(string customerId, string priceId, long amount)
    {
        var paymentIntentService = new PaymentIntentService();
        var paymentIntentOptions = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = "USD",
            Customer = customerId,
            PaymentMethodTypes = new List<string> { "card", "affirm", "amazon_pay", "paypal" },
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },

        };
        return await paymentIntentService.CreateAsync(paymentIntentOptions);

    }

    public async Task<SetupIntentDto> CreateCustomerAndSetupIntent(UserDto user)
    {
        try
        {
            var customerService = new CustomerService();
            var customerRetrieved = await customerService.ListAsync(new CustomerListOptions { Email = user.Email, Limit = 1 });
            Customer customer = customerRetrieved.Data.Count > 0
                                    ? customerRetrieved.Data[0]
                                    : await CreateCustomer(user);

            var priceId = GetPriceId(GetSubscriptionPlan(user.SubscriptionPlan ?? ""));
            var priceService = new PriceService();
            var price = await priceService.GetAsync(priceId);
            var paymentIntentService = new PaymentIntentService();
            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Customer = customer.Id,
                Amount = price.UnitAmount,
                Currency = "usd", // Set your currency
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
            };
            var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentOptions);
            return new SetupIntentDto
            {
                CustomerId = customer.Id,
                IntentClientSecret = paymentIntent.ClientSecret
            };
        }
        catch (StripeException)
        {
            throw new Exception("Stripe issues crashed the payment flow [$$-5]");
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }


    public async Task<Subscription> AttachPaymentMethodAndCreateSubscription
    (
        string customerId, // Customer ID
        string paymentMethodId, // Payment Method ID
        SubscriptionPlan subscriptionPlan // Subscription Plan
    )
    {
        try
        {
            var paymentMethodService = new PaymentMethodService();
            await paymentMethodService.AttachAsync(paymentMethodId, new PaymentMethodAttachOptions
            {
                Customer = customerId
            });

            var customerService = new CustomerService();
            var customer = await customerService.UpdateAsync(customerId, new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = paymentMethodId
                }
            });



            var subscriptionService = new SubscriptionService();
            var priceId = GetPriceId(subscriptionPlan) ?? throw new ArgumentNullException("Price ID not found.");
            var subscriptionsOptions = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId,
                    }
                },
                Expand = new List<string> { "latest_invoice.payment_intent" }
            };
            var subscription = await subscriptionService.CreateAsync(subscriptionsOptions);
            return subscription;
        }
        catch (StripeException)
        {
            throw new Exception("Stripe issues crashed the payment flow.");
        }
        catch (ArgumentNullException)
        {
            throw new Exception("Price ID not found. [$$-6]");
        }
    }


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

    public SubscriptionPlan GetSubscriptionPlan(string subcription)
    {
        SubscriptionPlan subscriptionPlan = subcription switch
        {
            "subscription-basic" => SubscriptionPlan.Basic,
            "subscription-standard" => SubscriptionPlan.Standard,
            "subscription-premium" => SubscriptionPlan.Premium,
            _ => throw new Exception("Invalid subscription plan.")
        };
        return subscriptionPlan;
    }

}
