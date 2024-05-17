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
                options.IdleTimeout = TimeSpan.FromSeconds(120);
                options.Cookie.IsEssential = true;
            });
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Add HttpContextAccessor
            services.AddScoped<SessionStorageService>();
            services.AddScoped<StripeRegistrationService>(sp => new StripeRegistrationService(configuration["Stripe:SecretKey"] ?? "Secret Stripe Key not provided", configuration));
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