namespace TmkGondorTreasury.DTOs;
using TmkGondorTreasury.DTOs.Enums;
/// <summary>
/// Represents a data transfer object for a payment intent.
/// </summary>
public record PaymentIntentDto
{
    public string? CustomerId { get; init; }

    public string? PaymentMethodId { get; init; }

    public UserDto? UserDto { get; init; }

    public SubscriptionPlan SubscriptionPlan { get; init; }
}
