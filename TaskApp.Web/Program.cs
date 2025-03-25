using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TaskApp.Data;
using TaskApp.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Definir cultura global para pt-BR
var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole>(options => {
    // Password settings for user registration
    options.Password.RequireDigit = false;       // No digits required for "Admin@ICAD!"
    options.Password.RequireLowercase = true;    // Require at least one lowercase letter
    options.Password.RequireUppercase = true;    // Require at least one uppercase letter
    options.Password.RequireNonAlphanumeric = true; // Require at least one special character
    options.Password.RequiredLength = 8;         // Minimum length is 8 characters
    
    // User settings
    options.User.RequireUniqueEmail = true;      // Email addresses must be unique
    options.User.AllowedUserNameCharacters = 
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Allow these characters in usernames
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});

var app = builder.Build();

// Create/migrate database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Initializing database...");
        
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        
        // Create database if it doesn't exist
        dbContext.Database.EnsureDeleted(); // Remove existing database to start fresh
        dbContext.Database.EnsureCreated();
        
        logger.LogInformation("Database created successfully");
        
        // Seed initial data
        await SeedData.InitializeAsync(services, logger);
        logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database: {Message}", ex.Message);
        
        if (ex.InnerException != null)
        {
            logger.LogError("Inner exception: {Message}", ex.InnerException.Message);
        }
    }
}

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
