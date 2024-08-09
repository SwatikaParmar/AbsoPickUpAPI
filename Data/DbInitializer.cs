using AnfasAPI.IServices;
using AnfasAPI.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext _context,
           UserManager<ApplicationUser> _userManager,
           RoleManager<IdentityRole> _roleManager,
           IFunctionalService _functionalService)
        {
            _context.Database.EnsureCreated();

            //check for users
            if (_context.ApplicationUser.Any())
            {
                return; //if user is not empty, DB has been seed
            }

            //init app with super admin user
            await _functionalService.CreateDefaultSuperAdmin();
        }
    }
}
