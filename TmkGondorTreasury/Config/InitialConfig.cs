using TmkGondorTreasury.Services;

namespace TmkGondorTreasury.Config
{
    public class InitialConfig
    {
        public static void ConfigureServices(IServiceCollection services)
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
            services.AddScoped<StripeRegistrationService>(sp => new StripeRegistrationService("sk_test_4eC39HqLyjWDarjtT1zdp7dc"));
            services.AddScoped<SessionStorageService>();
        }
    }
}