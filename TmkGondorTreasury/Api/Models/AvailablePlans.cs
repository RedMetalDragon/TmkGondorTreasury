using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Api.Models;

public class AvailablePlans
{
    public List<SubscriptionType>? MonthlyPlans { get; set; }
    public List<SubscriptionType>? YearlyPlans { get; set; }
}