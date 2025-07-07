// LoginCallbackModel.cs

using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Client.AspNetCore;

namespace Infra.Modules.AccountPanel.Pages.Auth.Login;

public class LoginCallbackModel : PageModel
{
    public bool Succeeded { get; private set; }
    public string? Json1 { get; private set; }
    public string? Json2 { get; private set; }

    public async Task OnGetAsync()
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        if (!result.Succeeded) 
        {
            Succeeded = false;
            return;
        }
        
        await HttpContext.SignInAsync(result.Principal!);
        
        var dto = new 
        {
            AuthenticationType = HttpContext.User.Identity?.AuthenticationType,
            Name               = HttpContext.User.Identity?.Name,
            IsAuthenticated    = HttpContext.User.Identity?.IsAuthenticated,
            Claims             = HttpContext.User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList()
        };
        
        Json1 = JsonSerializer.Serialize(
            dto,
            new JsonSerializerOptions { WriteIndented = true, ReferenceHandler    = ReferenceHandler.IgnoreCycles }
        );
        
        Json2 = JsonSerializer.Serialize(
            result.Properties,
            new JsonSerializerOptions { WriteIndented = true, ReferenceHandler    = ReferenceHandler.IgnoreCycles }
        );

        Succeeded = true;
    }
}