namespace TmkGondorTreasury.Services;
using Stripe;
using TmkGondorTreasury.DTOs;

public class StripeRegistrationService
{

    private readonly string _stripeSecretKey;

    public StripeRegistrationService(string stripeSecretKey)
    {
        _stripeSecretKey = stripeSecretKey;
        StripeConfiguration.ApiKey = _stripeSecretKey;
    }

    public async Task<PaymentIntent> CreatePaymentIntent(PaymentIntentDto paymentIntentDto)
    {
        try
        {
            var paymentIntent = new PaymentIntentCreateOptions
            {
                Amount = (long)paymentIntentDto.Amount,
                Currency = paymentIntentDto.Currency,
                PaymentMethod = paymentIntentDto.PaymentMethodId,
                ConfirmationMethod = "manual",
                Confirm = true,
                Metadata = new Dictionary<string, string>
                {
                    { "integration_check", "accept_a_payment" },
                    {"email", paymentIntentDto.UserDto?.Email ?? ""},
                    {"firstName", paymentIntentDto.UserDto?.FirstName ?? ""},
                    {"lastName", paymentIntentDto.UserDto?.LastName ?? ""},
                    {"subscriptionPlan", paymentIntentDto.UserDto?.SubsriptionPlan ?? ""}
                },
            };
            var service = new PaymentIntentService();
            return await service.CreateAsync(paymentIntent);
        }
        catch (NullReferenceException e)
        {
            throw new Exception(e.Message);
        }
        catch (StripeException e)
        {
            throw new Exception(e.Message);
        }
    }


}
