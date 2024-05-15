namespace TmkGondorTreasury.DTOs
{
    /// <summary>
    /// Represents a data transfer object for a payment intent.
    /// </summary>
    public record PaymentIntentDto
    {
        /// <summary>
        /// Gets or sets the amount of the payment intent in cents.
        /// </summary>
        public long Amount { get; init; }

        /// <summary>
        /// Gets or sets the currency of the payment intent.
        /// </summary>
        public string? Currency { get; init; }

        /// <summary>
        /// Gets or sets the ID of the payment method associated with the payment intent.
        /// </summary>
        public string? PaymentMethodId { get; init; }

        /// <summary>
        /// Gets or sets the user associated with the payment intent.
        /// </summary>
        public UserDto? UserDto { get; init; }
    }
}