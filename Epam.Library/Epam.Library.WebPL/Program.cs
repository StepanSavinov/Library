using System.Security.Claims;
using Epam.Library.DependencyConfig;
using Epam.Library.WebPL.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

Config.RegisterServices(builder.Services);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ActionFilter>();
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<AuthFilter>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Library/Error";
        options.LoginPath = "/User/Login/";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", builderClaims =>
    {
        builderClaims.RequireAssertion(a =>
            a.User.HasClaim(ClaimTypes.Role, "Admin") ||
            a.User.HasClaim(ClaimTypes.Role, "Librarian") ||
            a.User.HasClaim(ClaimTypes.Role, "User"));
    });
    options.AddPolicy("Librarian", builderClaims =>
    {
        builderClaims.RequireAssertion(a =>
            a.User.HasClaim(ClaimTypes.Role, "Librarian") ||
            a.User.HasClaim(ClaimTypes.Role, "Admin"));
    });
    options.AddPolicy("Admin", builderClaims =>
    {
        builderClaims.RequireClaim(ClaimTypes.Role, "Admin");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();