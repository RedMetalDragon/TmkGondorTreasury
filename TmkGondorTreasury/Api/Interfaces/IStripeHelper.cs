using Stripe;
using Stripe.Checkout;
using TmkGondorTreasury.Api.Models;

namespace TmkGondorTreasury.Api.Interfaces;

public interface IStripeHelper
{ 
    Task<IEnumerable<SubscriptionType>> GetSubscriptionsTypes();
    
    Task<Customer> CreateCustomer(string? email);
    
    Task<Subscription> CreateSubscription(string? customerId, string? priceId);
    
    Task<Stripe.Checkout.Session> CreateSession(string? email, string? customerId);
    
    Task<PaymentIntent> CreatePaymentIntent(Customer customer, Session session);
    
}