using Microsoft.EntityFrameworkCore;

namespace Aiursoft.DbTools.Tests;

public class MyDbContext : DbContext, ICanMigrate
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    // ReSharper disable once UnusedMember.Global
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public Task MigrateAsync(CancellationToken cancellationToken)
    {
        return Database.EnsureCreatedAsync(cancellationToken);
    }

    public Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}