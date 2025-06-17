using System.ComponentModel.DataAnnotations;

namespace TmkGondorTreasury.Api.Models;

public class CreateCheckoutSessionRequest
{
    [Required] [EmailAddress] public required string Email { get; set; }
    public required string PriceId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string FirstName { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string LastName { get; set; }
}