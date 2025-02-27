using Demo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.InMemory;

public class InMemoryContext(DbContextOptions<InMemoryContext> options) : DemoDbContext(options)
{
    public override Task MigrateAsync(CancellationToken cancellationToken)
    {
        return Database.EnsureCreatedAsync(cancellationToken);
    }

    public override Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}
