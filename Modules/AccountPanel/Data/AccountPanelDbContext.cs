using Microsoft.EntityFrameworkCore;

namespace Infra.Modules.AccountPanel.Data;

public class AccountPanelDbContext : DbContext
{
    public AccountPanelDbContext(DbContextOptions<AccountPanelDbContext> options)
        : base(options)
    {
    }
}