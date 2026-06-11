using Microsoft.AspNetCore.Identity;
using VetAmb.Models;

namespace VetAmb.Data
{
    public static class IdentitySeedData
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "Administrator", "Vet", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Identity role migration: legacy Owner role is deprecated and replaced by User.
            // Move users who only have Owner -> User, while preserving elevated roles.
            if (await roleManager.RoleExistsAsync("Owner"))
            {
                var ownerRoleUsers = await userManager.GetUsersInRoleAsync("Owner");
                foreach (var ownerRoleUser in ownerRoleUsers)
                {
                    if (!await userManager.IsInRoleAsync(ownerRoleUser, "User"))
                    {
                        await userManager.AddToRoleAsync(ownerRoleUser, "User");
                    }

                    await userManager.RemoveFromRoleAsync(ownerRoleUser, "Owner");
                }
            }

            // Ensure every account linked with Google has the User role.
            var allUsers = userManager.Users.ToList();
            foreach (var user in allUsers)
            {
                var logins = await userManager.GetLoginsAsync(user);
                var hasGoogleLogin = logins.Any(login =>
                    string.Equals(login.LoginProvider, "Google", StringComparison.OrdinalIgnoreCase));

                if (hasGoogleLogin && !await userManager.IsInRoleAsync(user, "User"))
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            const string adminEmail = "admin@vetamb.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    OIB = "12345678901",
                    JMBG = "1234567890123",
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, "AdminPassword123!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }

            const string vetEmail = "vet@vetamb.com";
            var vetUser = await userManager.FindByEmailAsync(vetEmail);

            if (vetUser == null)
            {
                vetUser = new AppUser
                {
                    UserName = vetEmail,
                    Email = vetEmail,
                    OIB = "22222222222",
                    JMBG = "2222222222222",
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(vetUser, "VetPassword123!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(vetUser, "Vet");
                }
            }
        }
    }
}