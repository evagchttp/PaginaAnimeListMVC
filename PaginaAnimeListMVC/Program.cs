using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PaginaAnimeListMVC.Data;
using PaginaAnimeListMVC.Models;
using PaginaAnimeListMVC.Services;
using PaginaAnimeListMVC.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()  // This line adds the necessary token providers.
    .AddSignInManager<SignInManager<ApplicationUser>>();  // SignInManager is automatically registered as well.

builder.Services.AddControllersWithViews();

// Add memory caching
builder.Services.AddMemoryCache();

// Agregar HttpClient a los servicios
builder.Services.AddHttpClient();
builder.Services.AddScoped<IJikanApiService, JikanApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Anime}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
