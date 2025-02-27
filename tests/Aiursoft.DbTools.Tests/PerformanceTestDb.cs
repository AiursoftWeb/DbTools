using Microsoft.EntityFrameworkCore;

namespace Aiursoft.DbTools.Tests;

public class PerformanceTestDb : DbContext
{
    public PerformanceTestDb(DbContextOptions<PerformanceTestDb> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
}