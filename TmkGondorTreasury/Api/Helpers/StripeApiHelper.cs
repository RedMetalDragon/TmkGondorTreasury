using Stripe;
using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Api.Helpers;


public class StripeApiHelper()
{
    public static async Task<IEnumerable<SubscriptionType>> GetSubscriptionsTypes()
    {
        var optionsPriceSearch = new PriceSearchOptions { Query = "active: true" };
        var optionsProductSearch = new ProductSearchOptions { Query = "active: true" };
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
                    SubscriptionDescription = product.Description ?? "",
                    SubscriptionCurrency = price.Currency ?? "USD",
                    SubscriptionBillingCycle = price.Recurring.Interval == "month"
                        ? SubscriptionBillingCycle.Monthly
                        : SubscriptionBillingCycle.Yearly
                }));
    }
}