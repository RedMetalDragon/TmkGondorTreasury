using TmkGondorTreasury.DTOs;
using System.Text.Json.Serialization;

namespace TmkGondorTreasury.Api.Models;

public class SubscriptionType
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionBillingCycle SubscriptionBillingCycle { get; set; }
    public long SubscriptionAmount { get; set; }
    
    public string? SubscriptionCurrency { get; set; } 
    public string? SubscriptionName { get; set; }
    
    public string? SubscriptionDescription { get; set; }
    public required string PriceId { get; set; }
}
