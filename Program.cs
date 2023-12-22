using App;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppQuizlet.Services;

using AppQuizlet.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
var appbuilder = new ConfigurationBuilder();
appbuilder.AddJsonFile("appsettings.json");
var config = appbuilder.Build();
builder.Services.AddIdentity<App.Models.User, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<SignInManager<App.Models.User>>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

	
	app.UseHsts();


	
}






app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
	options.RoutePrefix = string.Empty;
}); ;

app.UseHttpsRedirection();
app.UseStaticFiles();

TelegramFunction bot = new TelegramFunction(app.Services.CreateScope().ServiceProvider.GetService<SignInManager<App.Models.User>>(),app.Services.CreateScope().ServiceProvider.GetService<ApplicationContext>());




bot.bot();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
