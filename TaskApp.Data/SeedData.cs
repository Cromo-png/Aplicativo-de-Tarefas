using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskApp.Data.Models;

namespace TaskApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<User>>();

                // Create admin user if it doesn't exist
                var adminUser = await userManager.FindByNameAsync("admin");
                if (adminUser == null)
                {
                    // Ensure the admin user has a valid password that meets all requirements
                    // Password requirements:
                    // - At least one uppercase letter (A-Z)
                    // - At least one lowercase letter (a-z)
                    // - At least one non-alphanumeric character
                    // - At least 8 characters long
                    // - No digits required
                    adminUser = new User
                    {
                        UserName = "admin",
                        Email = "admin@taskapp.com",
                        FullName = "Administrator"
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@ICAD!");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Admin user created successfully.");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}", 
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Admin user already exists.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
} 