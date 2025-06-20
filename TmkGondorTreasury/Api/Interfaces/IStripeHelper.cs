using Stripe;
using Stripe.Checkout;
using TmkGondorTreasury.Api.Models;

namespace TmkGondorTreasury.Api.Interfaces;

public interface IStripeHelper
{
    Task<IEnumerable<SubscriptionType>> GetSubscriptionsTypes();

    Task<Customer> CreateCustomer(string? email);

    Task<Customer> CreateCustomer(string? email, string? fullName);

    Task<Subscription> CreateSubscription(string? customerId, string? priceId);
    
    Task<Subscription> CreateSubscription(string? customerId, string? priceId, string? clientTxnId );

    Task<Stripe.Checkout.Session> CreateSession(string? email, string? customerId);

    Task<PaymentIntent> CreatePaymentIntent(Customer customer, Session session);
}