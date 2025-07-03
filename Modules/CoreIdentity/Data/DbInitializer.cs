using Infra.Modules.CoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

namespace Infra.Modules.CoreIdentity.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync("admin@local");

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = "admin@local",
                Email = "admin@local",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, "Pass123$");
        }

        var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        if (await manager.FindByClientIdAsync("web-client") is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "web-client",
                DisplayName = "Web Client",
                RedirectUris = { new Uri("http://localhost:3000/callback") },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    "offline_access"
                }
            });
        }

        var scopeManager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();
        if (await scopeManager.FindByNameAsync("api") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api",
                DisplayName = "Default API access",
                Resources = { "resource_server" }
            });
        }
    }
}