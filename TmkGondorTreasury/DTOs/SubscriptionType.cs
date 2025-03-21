namespace TmkGondorTreasury.DTOs;

public class SubscriptionType
{
    public SubscriptionBillingCycle SubscriptionBillingCycle { get; set; }
    public long SubscriptionAmount { get; set; }
    
    public string? SubscriptionCurrency { get; set; } 
    public string? SubscriptionName { get; set; }
    
    public string? SubscriptionDescription { get; set; }
    public required string PriceId { get; set; }
}

public enum SubscriptionBillingCycle
{
    Monthly,
    Yearly
}