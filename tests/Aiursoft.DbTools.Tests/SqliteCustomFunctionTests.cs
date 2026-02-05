using System.Data;
using System.Data.Common;
using Aiursoft.DbTools.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.DbTools.Tests;

[TestClass]
public class SqliteCustomFunctionTests
{
    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options);

    [TestMethod]
    public async Task TestOnConnectionOpenCallback()
    {
        // Arrange
        var services = new ServiceCollection();
        bool callbackInvoked = false;
        
        services.AddAiurSqliteWithCache<TestDbContext>(
            connectionString: "DataSource=:memory:",
            allowCache: false,
            splitQuery: false,
            onConnectionOpen: (conn) =>
            {
                callbackInvoked = true;
                if (conn is SqliteConnection sqliteConn)
                {
                    // Verify we can register a function
                    sqliteConn.CreateFunction("TestFunc", (int x) => x * 2);
                }
            });

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var context = serviceProvider.GetRequiredService<TestDbContext>();
        
        // Ensure connection is opened
        await context.Database.OpenConnectionAsync();

        // Assert
        Assert.IsTrue(callbackInvoked, "The onConnectionOpen callback should have been invoked.");

        // Verify function works
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT TestFunc(21)";
        var result = await command.ExecuteScalarAsync();
        
        Assert.AreEqual(42, Convert.ToInt32(result));
    }
}
