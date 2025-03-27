namespace TmkGondorTreasury.Api.Models;

public class CreateCheckoutSessionResponse
{
    public required string SessionId { get; set; }
    public required string CustomerId { get; set; }
    public required string Currency { get; set; }
    public required string? IntentClientSecret {get;set;}
}