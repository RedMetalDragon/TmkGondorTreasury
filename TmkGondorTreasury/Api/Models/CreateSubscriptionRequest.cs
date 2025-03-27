using Stripe;

namespace TmkGondorTreasury.Api.Models;

public class CreateSubscriptionRequest
{
    public required string Email { get; set; }
    public required string PriceId { get; set; }
}