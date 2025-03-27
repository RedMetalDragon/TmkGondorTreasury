using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using TmkGondorTreasury;
using TmkGondorTreasury.Api;
using TmkGondorTreasury.Api.Interfaces;
using TmkGondorTreasury.Api.Services;
using TmkGondorTreasuryTest.Mocks;

namespace TmkGondorTreasuryTest.Setup
{
    // This test setup uses WebApplicationFactory to spin up an in-memory test server.
    // It overrides the dependency injection to replace the real IGondorConfigurationService with a mock.
    // Use the Client property to send HTTP requests and test your controller endpoints.
    // If you need to directly resolve controllers or other services, use the Services property.
    public class ControllersTestSetup : WebApplicationFactory<Program>
    {
        public HttpClient Client { get; private set; }

        // Expose the service provider to resolve dependencies directly if needed
        public override System.IServiceProvider Services => Server.Host.Services;

        public ControllersTestSetup()
        {
            // Create the test client from the in-memory server
            Client = this.CreateClient();
        }

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove any existing registration of IGondorConfigurationService
                var gondorDescriptor =
                    services.SingleOrDefault(d => d.ServiceType == typeof(IGondorConfigurationService));
                var stripeDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IStripeHelper));
                if (gondorDescriptor != null)
                {
                    services.Remove(gondorDescriptor);
                }

                if (stripeDescriptor != null)
                {
                    services.Remove(stripeDescriptor);
                }

                // Replace the service with our mocked implementation
                services.AddScoped<IGondorConfigurationService, MockGondorConfigService>();
                services.AddScoped<IStripeHelper, MockStripeHelper>();
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Client?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}