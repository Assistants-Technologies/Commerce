using Infra.Modules.CoreIdentity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Infra.Modules.CoreIdentity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; } = null!;
        public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; } = null!;
        public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; } = null!;
        public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; } = null!;
    }
}