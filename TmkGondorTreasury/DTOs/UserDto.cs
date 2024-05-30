namespace TmkGondorTreasury.DTOs;

public record UserDto
{
    public string? Email { get; init; }
    public string? Password { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public bool? AgreeOnTerms { get; init; }

    public string? SubscriptionPlan { get; init; }

}
