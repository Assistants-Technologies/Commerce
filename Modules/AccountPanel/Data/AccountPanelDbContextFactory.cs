using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infra.Modules.AccountPanel.Data;

public class AccountPanelDbContextFactory : IDesignTimeDbContextFactory<AccountPanelDbContext>
{
    public AccountPanelDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AccountPanelDbContext>();

        var connString = Environment.GetEnvironmentVariable("IDP_ACC_PANEL_CONNECTION_STRING")!;

        builder.UseNpgsql(connString);
        builder.UseOpenIddict();

        return new AccountPanelDbContext(builder.Options);
    }
}