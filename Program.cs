using App;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppQuizlet.Services;
using AppQuizlet.Services;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddEndpointsApiExplorer();
// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "sacsacas",
					  policy =>
					  {
						  policy.WithOrigins("http://localhost:5173").AllowCredentials().AllowAnyMethod().AllowAnyHeader();
					  });
});
builder.Services.Configure<IdentityOptions>(options =>
{
	// Default Password settings.
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 8;
	options.Password.RequiredUniqueChars = 0;
});
var appbuilder = new ConfigurationBuilder();
appbuilder.AddJsonFile("appsettings.json");
var config = appbuilder.Build();
builder.Services.AddIdentity<App.Models.User, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.SameSite = SameSiteMode.None;
});
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
app.UseCors("sacsacas");
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
