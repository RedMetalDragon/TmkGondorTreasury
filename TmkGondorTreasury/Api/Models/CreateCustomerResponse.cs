using Newtonsoft.Json;
using Stripe;

namespace TmkGondorTreasury.Api.Models;

public class CreateCustomerResponse
{
    [JsonProperty("customer")]
    public required Customer Customer { get; set; }
}