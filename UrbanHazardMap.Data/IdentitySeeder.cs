using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanHazardMap.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, string email, string password)
        {
            try
            {
                if (!await roleManager.RoleExistsAsync(RoleNames.Admin))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(RoleNames.Admin));
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception("Failed to create the admin role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
                var adminUser = await userManager.FindByEmailAsync(email);
                if (adminUser == null)
                {
                    adminUser = new IdentityUser { UserName = email, Email = email , EmailConfirmed = true };
                    var result = await userManager.CreateAsync(adminUser, password);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                    else
                    {
                        Console.WriteLine($"Admin user created with email: {email}");
                    }
                }
                if (!await userManager.IsInRoleAsync(adminUser, RoleNames.Admin))
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception("Failed to add the admin user to the admin role: " + string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding roles and admin user: {ex.Message}");
            }
        }
    }
}
