using TmkGondorTreasury.Api.Services;

namespace TmkGondorTreasuryTest.Mocks;

public class MockGondorConfigService : IGondorConfigurationService
{
    public string GetConfigurationValue(string key)
    {
        // read file from .env file
        var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        var envVariables = File.ReadAllLines(envFilePath)
            .Select(line => line.Split('='))
            .ToDictionary(parts => parts[0], parts => parts[1]);
        return key switch
        {
            "Stripe:SecretKey" => envVariables["StripeSecretKey"],
            _ => throw new Exception("Key not found")
        };
    }
}