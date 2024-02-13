using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
//add services into loc container
builder.Services.AddSingleton<IPersonsService,PersonsService>();
builder.Services.AddSingleton<ICountriesServices, CounriesService>();
var app = builder.Build();
if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();   

app.Run();
