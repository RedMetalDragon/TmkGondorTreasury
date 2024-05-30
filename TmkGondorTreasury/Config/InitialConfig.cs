using TmkGondorTreasury.Services;

namespace TmkGondorTreasury.Config
{
    public static class InitialConfig
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddDistributedMemoryCache();
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
                            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                            ;
                    });
            });
            services.AddScoped<SessionStorageService>();
            services.AddScoped<StripeRegistrationService>(sp => new StripeRegistrationService(configuration["Stripe:ApiKey"] ?? "Secret Stripe Key not provided", configuration));
        }
        public static void ConfigureApp(this WebApplication webApplication)
        {
            //var stripeConfig = webApplication.Configuration.GetSection("Stripe").Get<StripeConfig>();
            // stripe key set as secret
            if (webApplication.Environment.IsDevelopment())
            {

            }
        }
        public static void ConfigureMiddleware(IApplicationBuilder app)
        {
            // Implement 

        }
    }
}