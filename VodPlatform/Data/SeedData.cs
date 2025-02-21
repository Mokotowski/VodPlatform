using Microsoft.AspNetCore.Identity;
using VodPlatform.Database;

namespace VodPlatform.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, RoleManager<IdentityRole> roleManager, UserManager<UserModel> userManager)
        {
            var roles = new[] { "SuperAdmin", "Admin", "Moderator", "Series", "Movies", "User" };

            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }


            var user = await userManager.FindByEmailAsync("admin@admin.com");
            if (user == null)
            {
                user = new UserModel
                {
                    UserName = "Admin",
                    Email = "admin@admin.com",
                    Firstname = "Admin",
                    Lastname = "Admin"
                };

                var result = await userManager.CreateAsync(user, "12345678");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "SuperAdmin");
                }
            }
            else
            {
                var rolesList = await userManager.GetRolesAsync(user);
                if (!rolesList.Contains("SuperAdmin"))
                {
                    await userManager.AddToRoleAsync(user, "SuperAdmin");
                }
            }
        }
    }
}
