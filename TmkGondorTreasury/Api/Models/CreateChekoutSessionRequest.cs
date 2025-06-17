namespace TmkGondorTreasury.Api.Models;

public class CreateCheckoutSessionRequest
{
    public string Email { get; set; }
    public string PriceId { get; set; }
}