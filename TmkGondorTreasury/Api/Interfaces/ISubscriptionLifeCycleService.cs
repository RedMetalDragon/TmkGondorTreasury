using Stripe;
namespace TmkGondorTreasury.Api.Interfaces;

public interface ISubscriptionLifeCycleService
{
    Task HandleSubscriptionCreated(Event subscriptionCreatedEvent);
    void HandleSubscriptionDeleted();
    void HandleSubscriptionUpdated();
}