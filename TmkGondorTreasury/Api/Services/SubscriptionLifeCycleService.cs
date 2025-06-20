using Stripe;
using TmkGondorTreasury.Api.Interfaces;
using TmkRabbitMqLibrary.Publisher.Interfaces;

namespace TmkGondorTreasury.Api.Services;

public class SubscriptionLifeCycleService : ISubscriptionLifeCycleService
{
    private readonly ILogger<SubscriptionLifeCycleService> _logger;
    private readonly ITmkRabbitMqPublisher _rabbitMqPublisher;
    private readonly string routinKeyForStripeSubscriptions = "stripe.customer.subscription.created";
    private readonly string exchangeNameForStripeSubscriptions = "tmk-core-events";

    public SubscriptionLifeCycleService(ILogger<SubscriptionLifeCycleService> logger,
        ITmkRabbitMqPublisher rabbitMqPublisher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rabbitMqPublisher = rabbitMqPublisher ?? throw new ArgumentNullException(nameof(rabbitMqPublisher));
    }

    public Task HandleSubscriptionCreated(Event subscriptionCreatedEvent)
    {
        _logger.LogInformation("Subscription created");
        return Task.CompletedTask;
    }

    public void HandleSubscriptionDeleted()
    {
        throw new NotImplementedException();
    }

    public void HandleSubscriptionUpdated()
    {
        throw new NotImplementedException();
    }
}