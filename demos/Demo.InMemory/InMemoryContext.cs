using Demo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.InMemory;

public class InMemoryContext(DbContextOptions<InMemoryContext> options) : DemoDbContext(options)
{
    public override async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await Database.EnsureDeletedAsync(cancellationToken);
        await Database.EnsureCreatedAsync(cancellationToken);
    }

    public override Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}
