using Stripe;
namespace TmkGondorTreasury.Api.Interfaces;

public interface ISubscriptionLifeCycleService
{
    Task HandleInvoicePaymentSuccess(Event invoicePaymentSuccessEvent);
    Task HandleSubscriptionCreated(Event subscriptionCreatedEvent);
}