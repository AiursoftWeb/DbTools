using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.DbTools.Tests;

public class PerformanceTestDb : DbContext
{
    public PerformanceTestDb(DbContextOptions<PerformanceTestDb> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
}

[TestClass]
public class PerformanceTest
{
    private const string Sqlite = "DataSource=app.db;Cache=Shared";
    
    [TestMethod]
    public async Task TestSqliteDefaultPool()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContextPool<PerformanceTestDb>(optionsBuilder =>
            optionsBuilder.UseSqlite(Sqlite));
        var built = services.BuildServiceProvider();
        var db = built.GetRequiredService<PerformanceTestDb>();
        await TestDb(db);
    }
    
    [TestMethod]
    public async Task TestSqliteDefaultContext()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<PerformanceTestDb>(optionsBuilder =>
            optionsBuilder.UseSqlite(Sqlite));
        var built = services.BuildServiceProvider();
        var db = built.GetRequiredService<PerformanceTestDb>();
        await TestDb(db);
    }
    
    [TestMethod]
    public async Task TestMemoryDefaultContext()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInMemoryDb<PerformanceTestDb>();
        var built = services.BuildServiceProvider();
        var db = built.GetRequiredService<PerformanceTestDb>();
        await TestDb(db);
    }

    private async Task TestDb(PerformanceTestDb db)
    {
        var totalWatch = new Stopwatch();
        totalWatch.Start();

        await RunWithWatch(async () =>
        {
            try
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
                await db.Database.MigrateAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }, "Database created!");
        
        await RunWithWatch(async () =>
        {
            for (int i = 0; i < 3000; i++)
            {
                db.Books.Add(new Book
                {
                    Name = Guid.NewGuid().ToString()
                });
                await db.SaveChangesAsync();
            }
        }, "Database inserted 3000 items!");

        await RunWithWatch(async () =>
        {
            for (int i = 0; i < 10000; i++)
            {
                var results = await db.Books.ToListAsync();
                _ = results.Count;
            }
        }, "Database tolisted 10000 times!");

        await RunWithWatch(async () =>
        {
            for (int i = 0; i < 1000; i++)
            {
                var results = await db.Books.Where(b => b.Name.Contains(i.ToString())).ToListAsync();
                _ = results.Count;
            }
        }, "Database queried with where condition 1000 times!");
        
        await RunWithWatch(async () =>
        {
            for (int i = 0; i < 700; i++)
            {
                // Insert.
                db.Books.Add(new Book
                {
                    Name = Guid.NewGuid().ToString()
                });
                await db.SaveChangesAsync();
                
                // Then query
                var results = await db.Books.Where(b => b.Name.Contains(i.ToString())).ToListAsync();
                _ = results.Count;
            }
        }, "Database inserted-then-queried with condition 700 times!");
        
        await RunWithWatch(async () =>
        {
            for (int i = 0; i < 500; i++)
            {
                // Insert.
                db.Books.Remove(await db.Books.FirstAsync());
                await db.SaveChangesAsync();
                
                // Then query
                var results = await db.Books.Where(b => b.Name.Contains(i.ToString())).ToListAsync();
                _ = results.Count;
            }
        }, "Database delete-then-queried with condition 500 times!");

        totalWatch.Stop();
        Console.WriteLine(totalWatch.Elapsed + " Totally elapsed!");
        
        await db.Database.EnsureDeletedAsync();
    }

    private async Task RunWithWatch(Func<Task> taskFactory, string whatDone)
    {
        var watch = new Stopwatch();
        watch.Start();
        await taskFactory();
        watch.Stop();
        Console.WriteLine(watch.Elapsed + " " + whatDone);
    }
}