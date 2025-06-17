namespace TmkGondorTreasury.DTOs
{
    public record SubscriptionCreateResponse
    {
        public string? ClientSecret { get; init; }
        public string? SubscriptionId { get; init; }
        public string? CustomerId { get; set; }

        public string? Error { get; init; }
    }
}