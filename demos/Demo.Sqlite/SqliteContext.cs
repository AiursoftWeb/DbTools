using Demo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.Sqlite;

public class SqliteContext(DbContextOptions<SqliteContext> options) : DemoDbContext(options)
{
    public override Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}