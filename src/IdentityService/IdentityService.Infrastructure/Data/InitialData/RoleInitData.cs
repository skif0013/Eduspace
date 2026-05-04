using IdentityService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Database.InitialData;

public class RoleInitData
{
    public static async Task InitializeAsync(RoleManager<RoleIdentity> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))  // TODO !!!
        {
            var adminRole = new RoleIdentity{ Name = "Admin", NormalizedName = "ADMIN" };
            await roleManager.CreateAsync(adminRole);
        }
        
        if (!await roleManager.RoleExistsAsync("User"))    // TODO !!!
        {
            var adminRole = new RoleIdentity{ Name = "User", NormalizedName = "USER" };
            await roleManager.CreateAsync(adminRole);
        }
    }
}