using Stripe;
using Stripe.Checkout;
using TmkGondorTreasury.Api.Interfaces;
using TmkGondorTreasury.Api.Models;
using TmkGondorTreasury.Api.Services;
using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Api.Helpers;

public class StripeApiHelper : IStripeHelper
{
    const string MetadataProductDescription = "product_description";
    const string KeyForStripeKey = "Stripe:APIKEY";
    private readonly ILogger<StripeApiHelper> _logger;

    public StripeApiHelper(IGondorConfigurationService configuration, ILogger<StripeApiHelper> logger)
    {
        _logger = logger;
        StripeConfiguration.ApiKey = configuration.GetConfigurationValue(KeyForStripeKey);
    }

    public async Task<IEnumerable<SubscriptionType>> GetSubscriptionsTypes()
    {
        var optionsPriceSearch = new PriceSearchOptions { Query = "active: 'true'" };
        var optionsProductSearch = new ProductSearchOptions { Query = "active: 'true'" };
        var priceService = new PriceService();
        var productService = new ProductService();
        var priceTask = priceService.SearchAsync(optionsPriceSearch);
        var productTask = productService.SearchAsync(optionsProductSearch);
        var prices = await priceTask;
        var products = await productTask;
        return products.SelectMany(product =>
            prices.Where(price => price.ProductId == product.Id)
                .Select(price => new SubscriptionType
                {
                    PriceId = price.Id,
                    SubscriptionAmount = price.UnitAmount ?? 0,
                    SubscriptionName = product.Name,
                    SubscriptionDescription =
                        product.Metadata.TryGetValue(MetadataProductDescription, out var description)
                            ? description
                            : string.Empty,
                    SubscriptionCurrency = price.Currency ?? "USD",
                    SubscriptionBillingCycle = price.Recurring.Interval == "month"
                        ? SubscriptionBillingCycle.Monthly
                        : SubscriptionBillingCycle.Yearly
                }));
    }

    public async Task<Customer> CreateCustomer(string? email)
    {
        var options = new CustomerCreateOptions
        {
            Email = email
        };
        var service = new CustomerService();
        return await service.CreateAsync(options);
    }

    public Task<Customer> CreateCustomer(string? email, string? fullName)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = fullName,
            Description = "Root account user"
        };
        var service = new CustomerService();
        return service.CreateAsync(options);
    }

    public async Task<Subscription> CreateSubscription(string? customerId, string? priceId)
    {
        try
        {
            // Create subscription
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId,
                    },
                },
                PaymentBehavior = "default_incomplete",
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");
            var subscriptionService = new SubscriptionService();

            return await subscriptionService.CreateAsync(subscriptionOptions);
        }
        catch (StripeException e)
        {
            _logger.Log(LogLevel.Error, e, $"--Error creating subscription for customer: <{customerId}> --");
            throw;
        }
    }

    public async Task<Subscription> CreateSubscription(string? customerId, string? priceId, string? clientTxnId)
    {
        try
        {
            var metadata = new Dictionary<string, string>();
            if (clientTxnId != null) 
                metadata.Add("txnId", clientTxnId);
            // Create subscription
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId,
                    },
                },
                PaymentBehavior = "default_incomplete",
                Metadata = metadata
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");
            var subscriptionService = new SubscriptionService();

            return await subscriptionService.CreateAsync(subscriptionOptions);
        }
        catch (StripeException e)
        {
            _logger.Log(LogLevel.Error, e, $"--Error creating subscription for customer: <{customerId}> --");
            throw;
        }
    }

    public async Task<Session> CreateSession(string? customerId, string? priceId)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                Customer = customerId,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1
                    }
                },
                Mode = "subscription",
                SuccessUrl = "https://example.com/success",
                Expand = new List<string> { "payment_intent" }
            };
            var service = new SessionService();
            return await service.CreateAsync(options);
        }
        catch (StripeException e)
        {
            _logger.Log(LogLevel.Error, e, $"--Error creating checkout session for customer: <{customerId}> --");
            throw;
        }
    }

    public Task<PaymentIntent> CreatePaymentIntent(Customer customer, Session session)
    {
        var options = new PaymentIntentCreateOptions
        {
            Customer = customer.Id,
            Amount = session.AmountTotal,
            Currency = session.Currency,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            }
        };
        var service = new PaymentIntentService();
        return service.CreateAsync(options);
    }
}