using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using TmkGondorTreasuryTest.Setup;

namespace TmkGondorTreasuryTest.Tests
{
    // This test class uses the ControllersTestSetup to run integration tests against the PlansController endpoints.
    // Adjust the endpoint paths based on your API routing configuration.
    public class PlansControllerTest : IClassFixture<ControllersTestSetup>
    {
        private readonly HttpClient _client;

        public PlansControllerTest(ControllersTestSetup factory)
        {
            _client = factory.Client;
        }

        [Fact]
        public async Task GetPlans_ReturnsOkStatus()
        {
            // Assuming your PlansController has a GET endpoint at /plans
            var response = await _client.GetAsync("/plans");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetPlans_ReturnsNonEmptyContent()
        {
            var response = await _client.GetAsync("/plans");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(content), "Response content should not be empty.");
        }
    }
}