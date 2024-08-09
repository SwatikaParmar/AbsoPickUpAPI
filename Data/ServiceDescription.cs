using AnfasAPI.IServices;
using AnfasAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Data
{
    public static class ServiceDescription
    {
        public static IHost MigrateDbContext<TContext>(this IHost host) where TContext : DbContext
        {
            // Create a scope to get scoped services.
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var netcoreService = services.GetRequiredService<IFunctionalService>();
                    DbInitializer.Initialize(context, userManager, roleManager, netcoreService).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<TContext>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }

                //Added
                //var logger = services.GetRequiredService<ILogger<TContext>>();
                // get the service provider and db context.
                //var context = services.GetService<TContext>();
                // do something you can customize.
                // For example, I will migrate the database.
                //context.Database.Migrate();
            }

            return host;
        }
    }
}
