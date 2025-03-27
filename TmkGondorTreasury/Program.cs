using Serilog;
using TmkGondorTreasury.Config;

namespace TmkGondorTreasury;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
        );

        // Add services to the container.
        InitialConfig.ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<Serilog.ILogger>();
        InitialConfig.LogSelectedConfigurationValues(builder.Configuration, logger);
        // Configure the HTTP request pipeline.
        app.UseCors();
        app.UseRouting();
        app.UseSession();
        app.UseHttpsRedirection();
        app.UseEndpoints(endpoints => { _ = endpoints.MapControllers(); });
        app.MapControllers();
        app.Run();
    }
}