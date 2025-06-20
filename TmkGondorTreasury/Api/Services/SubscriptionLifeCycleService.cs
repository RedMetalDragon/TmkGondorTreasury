using Stripe;
using TmkGondorTreasury.Api.Interfaces;
using TmkRabbitMqLibrary.Publisher.Interfaces;

namespace TmkGondorTreasury.Api.Services;

public class SubscriptionLifeCycleService : ISubscriptionLifeCycleService
{
    private readonly ILogger<SubscriptionLifeCycleService> _logger;
    private readonly ITmkRabbitMqPublisher _rabbitMqPublisher;
    private readonly string routinKeyForStripeSubscriptionsCreated = "stripe.customer.subscription.created";
    private readonly string routingKeyForStripeInvoicesSucceeded = "stripe.customer.subscription.invoices.succeeded";
    private readonly string exchangeNameForCoreEvents = "tmk-core-events";

    public SubscriptionLifeCycleService(ILogger<SubscriptionLifeCycleService> logger,
        ITmkRabbitMqPublisher rabbitMqPublisher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rabbitMqPublisher = rabbitMqPublisher ?? throw new ArgumentNullException(nameof(rabbitMqPublisher));
    }

    public Task HandleInvoicePaymentSuccess(Event invoicePaymentSuccessEvent)
    {
        throw new NotImplementedException();
    }

    public Task HandleSubscriptionCreated(Event subscriptionCreatedEvent)
    {
        _logger.LogInformation("Subscription created");
        return Task.CompletedTask;
    }
}