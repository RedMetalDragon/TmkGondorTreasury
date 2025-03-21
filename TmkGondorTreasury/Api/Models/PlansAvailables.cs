using TmkGondorTreasury.DTOs;

namespace TmkGondorTreasury.Api.Models;

public class PlansAvailables
{
    public List<SubscriptionType>? MonthlyPlans { get; set; }
    public List<SubscriptionType>? YearlyPlans { get; set; }
}