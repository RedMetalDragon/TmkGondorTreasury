namespace TmkGondorTreasury.DTOs;

public record SetupIntentDto
{

    public string? CustomerId { get; init; }
    public string? IntentClientSecret{ get; init; }
    public string? Error { get; init; }
}