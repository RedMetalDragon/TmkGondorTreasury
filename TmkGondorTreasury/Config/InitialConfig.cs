using TmkGondorTreasury.Api.Helpers;
using TmkGondorTreasury.Api.Interfaces;
using TmkGondorTreasury.Api.Services;

namespace TmkGondorTreasury.Config
{
    public static class InitialConfig
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IGondorConfigurationService, GondorConfigurationService>();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".TmkGondorTreasury.Session";
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.IsEssential = true;
            });
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
                    });
            });
            services.AddScoped<SessionStorageService>();
            services.AddScoped<IStripeHelper, StripeApiHelper>();
            services.AddScoped<StripeRegistrationService>(sp =>
            {
                var configService = sp.GetRequiredService<IGondorConfigurationService>();
                string stripeApiKey = configService.GetConfigurationValue("Stripe:Api:Key") ?? "Secret Stripe Key not provided";
                return new StripeRegistrationService(stripeApiKey, configService, sp.GetRequiredService<ILogger<StripeRegistrationService>>());
            });
        }

        public static void ConfigureApp(this WebApplication webApplication)
        {


        }

        public static void ConfigureMiddleware(IApplicationBuilder app)
        {
            // Implement 

        }

        public static void LogSelectedConfigurationValues(IConfiguration configuration, Serilog.ILogger logger)
        {
            var keysToLog = new List<string>
            {
                "STRIPE_APIKEY", 
                // Assuming you want to check it's loaded but not log the actual key
                //"SomeOtherConfigKey",
                //"YetAnotherConfigKey"
            };

            foreach (var key in keysToLog)
            {
                var value = configuration[key];
                if (!string.IsNullOrEmpty(value))
                {
                    // Log a masked value or a confirmation that the key is present, but not the actual value
                    logger.Information("{Key} is configured.", key);
                }
                else
                {
                    logger.Error("{Key} is not configured.", key);
                }
            }
        }

    }
}