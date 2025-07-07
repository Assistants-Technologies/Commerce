using Infra.Modules.AccountPanel.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AccountPanelDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("IDP_ACC_PANEL_CONNECTION_STRING")!);
    options.UseOpenIddict();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
    options.SlidingExpiration = false;
});

builder.Services.AddOpenIddict(opt =>
{
    opt.AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<AccountPanelDbContext>();
    });
    
    opt.AddClient(options =>
    {
        options.UseSystemNetHttp();

        options.AllowAuthorizationCodeFlow();

        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
            .EnableStatusCodePagesIntegration()
            .EnableRedirectionEndpointPassthrough()
            .EnablePostLogoutRedirectionEndpointPassthrough();

        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri(Environment.GetEnvironmentVariable("IDP_ISSUER") ??
                             throw new InvalidOperationException("IDP_ISSUER environment variable is not set.")),
            ClientId = Environment.GetEnvironmentVariable("IDP_ACC_PANEL_CLIENT_ID") ??
                       throw new InvalidOperationException("IDP_ACC_PANEL_CLIENT_ID environment variable is not set."),
            ClientSecret = Environment.GetEnvironmentVariable("IDP_ACC_PANEL_CLIENT_SECRET") ??
                           throw new InvalidOperationException(
                               "IDP_ACC_PANEL_CLIENT_SECRET environment variable is not set."),
            RedirectUri = new Uri(Environment.GetEnvironmentVariable("IDP_ACC_PANEL_REDIRECT_URI") ??
                                  throw new InvalidOperationException(
                                      "IDP_ACC_PANEL_REDIRECT_URI environment variable is not set.")),
            PostLogoutRedirectUri =
                new Uri(Environment.GetEnvironmentVariable("IDP_ACC_PANEL_POST_LOGOUT_REDIRECT_URI") ??
                        throw new InvalidOperationException(
                            "IDP_ACC_PANEL_POST_LOGOUT_REDIRECT_URI environment variable is not set.")),
            Scopes = { "openid", "profile", "email", "api:store" }
        });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccountPanelDbContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await app.RunAsync();