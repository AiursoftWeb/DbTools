using Aiursoft.DbTools;
using Microsoft.EntityFrameworkCore;

namespace Demo.Entities;

public abstract class DemoDbContext(DbContextOptions options) : DbContext(options), ICanMigrate
{
    public DbSet<Author> Authors => Set<Author>();

    public DbSet<Book> Books => Set<Book>();

    public virtual  Task MigrateAsync(CancellationToken cancellationToken) =>
        Database.MigrateAsync(cancellationToken);

    public virtual  Task<bool> CanConnectAsync() =>
        Database.CanConnectAsync();
}