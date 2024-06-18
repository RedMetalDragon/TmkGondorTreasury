using Microsoft.AspNetCore.Mvc;
using TmkGondorTreasury.Config;
using TmkGondorTreasury.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
InitialConfig.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
var list = System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);
foreach (var item in list.Keys)
{
    Console.WriteLine($"{item} : {list[item]}");
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}
app.UseCors();
app.UseRouting();
app.UseSession();
app.UseHttpsRedirection();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});
app.MapControllers();
app.Run();
