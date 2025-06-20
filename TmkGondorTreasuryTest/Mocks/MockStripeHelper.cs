using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using TmkGondorTreasury.Api.Interfaces;
using TmkGondorTreasury.Api.Models;
using TmkGondorTreasury.Api.Services;
using TmkGondorTreasury.DTOs;
using File = System.IO.File;

namespace TmkGondorTreasuryTest.Mocks;

public class MockStripeHelper : IStripeHelper
{
    public async Task<IEnumerable<SubscriptionType>> GetSubscriptionsTypes()
    {
        // Path to the JSON file located in Mocks/Files folder
        var filePathForJsonProduct = Path.Combine(AppContext.BaseDirectory, "Mocks", "Files",
            "search_products_stripe_response.json");
        var filePathForJsonPrice =
            Path.Combine(AppContext.BaseDirectory, "Mocks", "Files", "search_prices_stripe_response.json");

        if (!File.Exists(filePathForJsonProduct))
        {
            throw new FileNotFoundException("Mock JSON file not found.", filePathForJsonProduct);
        }

        if (!File.Exists(filePathForJsonPrice))
        {
            throw new FileNotFoundException("Mock JSON file not found.", filePathForJsonPrice);
        }

        var jsonContentForProductSearch = await File.ReadAllTextAsync(filePathForJsonProduct);
        var jsonContentForPriceSearch = await File.ReadAllTextAsync(filePathForJsonPrice);
        var products = JsonConvert.DeserializeObject<StripeSearchResult<Product>>(jsonContentForProductSearch);
        var prices = JsonConvert.DeserializeObject<StripeSearchResult<Price>>(jsonContentForPriceSearch);

        // Map each product from the JSON to a SubscriptionType
        // Here, we use dummy logic: if the product name contains "Premium", mark it as Monthly, otherwise Yearly
        var subscriptionTypes = products.SelectMany(product =>
            prices.Where(price => price.ProductId == product.Id)
                .Select(price => new SubscriptionType
                {
                    PriceId = price.Id,
                    SubscriptionAmount = price.UnitAmount ?? 0,
                    SubscriptionName = product.Name,
                    SubscriptionDescription =
                        product.Metadata.TryGetValue("product_description", out var description)
                            ? description
                            : string.Empty,
                    SubscriptionCurrency = price.Currency ?? "USD",
                    SubscriptionBillingCycle = price.Recurring.Interval == "month"
                        ? SubscriptionBillingCycle.Monthly
                        : SubscriptionBillingCycle.Yearly
                }));
        return subscriptionTypes;
    }

    public Task<Customer> CreateCustomer(string? email)
    {
        //TODO: Implement this method
        throw new NotImplementedException();
    }

    public Task<Customer> CreateCustomer(string? email, string? fullName)
    {
        //TODO: Implement this method
        throw new NotImplementedException();
    }

    public Task<Subscription> CreateSubscription(string? customerId, string? priceId)
    {
        //TODO: Implement this method
        throw new NotImplementedException();
    }

    public Task<Subscription> CreateSubscription(string? customerId, string? priceId, string? clientTxnId)
    {
        throw new NotImplementedException();
    }

    public Task<Session> CreateSession(string? email, string? customerId)
    {
        throw new NotImplementedException();
    }

    public Task<PaymentIntent> CreatePaymentIntent(Customer customer, Session session)
    {
        throw new NotImplementedException();
    }

    public Task<PaymentIntent> CreatePaymentIntent(Customer customer, string priceId)
    {
        throw new NotImplementedException();
    }
}