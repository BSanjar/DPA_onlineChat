using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using onlineChat2;
using onlineChat2.Models;
using onlineChat2.Models.DB_Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();


//внедрение зависимости  в контроллер
builder.Services.AddDbContext<FeedbackContext>(
	options =>
	{
		options
		.UseLoggerFactory(LoggerFactory.Create(builder => { }))
		.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
	});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => //CookieAuthenticationOptions
	{
		options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/User/Login");
	});

builder.Services.Configure<appSettings>(builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();    // аутентификация
app.UseAuthorization();     // авторизация

app.MapHub<ChatHub>("/chat");   // ChatHub будет обрабатывать запросы по пути /chat


app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=newChat}/{id?}");

app.Run();
