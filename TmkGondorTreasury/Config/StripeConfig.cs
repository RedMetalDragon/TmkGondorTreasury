namespace TmkGondorTreasury.Config;
public class StripeConfig
{
    public string ApiKey { get; set; }
    public string SecretKey { get; set; }
    public string WebhookSecret { get; set; }
    // Add more properties as needed

    public StripeConfig()
    {
        // Initialize properties with default values or read from secrets/env variables
        // ApiKey = GetSecretOrEnvVariable("StripeApiKey");
        // SecretKey = GetSecretOrEnvVariable("StripeSecretKey");
        // WebhookSecret = GetSecretOrEnvVariable("StripeWebhookSecret");
    }

    private string GetSecretOrEnvVariable(string name)
    {
        // Logic to read the secret or env variable value
        // You can use a library like SecretManager or directly access environment variables
        // Replace the following line with your own implementation
        return Environment.GetEnvironmentVariable(name);
    }
}