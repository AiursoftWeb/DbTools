using Aiursoft.DbTools.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.DbTools.Tests;

[TestClass]
public class EntityFrameworkExtendsTests
{
    [TestMethod]
    public async Task TestSync()
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        hostBuilder.ConfigureServices(services =>
            services.AddAiurSqliteWithCache<MyDbContext>("Data Source=app.db")
        );
        var host = hostBuilder.Build();
        await host.UpdateDbAsync<MyDbContext>();
        // Arrange
        var context = host.Services.CreateScope().ServiceProvider.GetRequiredService<MyDbContext>();
        var dbSet = context.Books;

        var collection = new List<Book>
        {
            new() { Name = "Book 1" },
            new() { Name = "Book 2" },
            new() { Name = "Book 3" }
        };

        dbSet.Sync(t => true, collection);
        await context.SaveChangesAsync();
        var results = await context.Books.ToListAsync();

        Assert.AreEqual(3, results.Count);
        Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 1"));
        Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 2"));
        Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 3"));

        collection.RemoveAt(1);
        collection.Add(new Book { Name = "Book 4" });

        dbSet.Sync(t => true, collection);
        await context.SaveChangesAsync();
        results = await context.Books.ToListAsync();

        Assert.AreEqual(3, dbSet.Count());
        Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 1"));
        Assert.IsNull(results.SingleOrDefault(t => t.Name == "Book 2"));
        Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 3"));
        Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 4"));
    }
}