using Microsoft.AspNetCore.Mvc;
using Stripe;
using TmkGondorTreasury.Api.Helpers;
using TmkGondorTreasury.Api.Models;
using TmkGondorTreasury.Api.Services;

namespace TmkGondorTreasury.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : Controller
{
    private readonly IGondorConfigurationService _configuration;

    public CustomerController(IGondorConfigurationService configuration)
    {
        _configuration = configuration;
    }
    
    [HttpPost("create-customer")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest customerRequest)
    {
        var options = new CustomerCreateOptions
        {
            Email = customerRequest.Email
        };
        var service = new CustomerService();
        var customer = await service.CreateAsync(options);
        var response = new CreateCustomerResponse
        {
            Customer = customer
        };
        return Ok(response);
    }
}