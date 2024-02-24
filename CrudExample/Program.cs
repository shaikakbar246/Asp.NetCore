using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using Repositories;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);
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

Rotativa.AspNetCore.RotativaConfiguration.Setup
    ("wwwroot", wkhtmltopdfRelativePath: "Rotativa");//generate pdf
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();   

app.Run();
