using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using Repositories;
using RepositoryContracts;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});
//Logging
builder.Host.ConfigureLogging(loggingProvider => {
    loggingProvider.ClearProviders();
    loggingProvider.AddConsole();
    loggingProvider.AddDebug();
    loggingProvider.AddEventLog();
});
builder.Services.AddControllersWithViews();
//add services into loc container
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

builder.Services.AddScoped<IPersonsService,PersonsService>();
builder.Services.AddScoped<ICountriesServices, CounriesService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();
if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");
Rotativa.AspNetCore.RotativaConfiguration.Setup
    ("wwwroot", wkhtmltopdfRelativePath: "Rotativa");//generate pdf
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();   

app.Run();
