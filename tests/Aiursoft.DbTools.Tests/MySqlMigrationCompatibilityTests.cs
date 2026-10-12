using Demo.MySql;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.DbTools.Tests;

[TestClass]
public class MySqlMigrationCompatibilityTests
{
    [TestMethod]
    [DataRow(8, 4, 0)]
    [DataRow(9, 5, 0)]
    [DataRow(9, 7, 0)]
    public void HistoricalSnapshotMatchesModel(int major, int minor, int patch)
    {
        // The demo retains its EF Core 9 migrations and snapshot. A provider
        // upgrade must not make its existing schema require a new migration.
        var options = new DbContextOptionsBuilder<MySqlContext>()
            .UseMySql(
                "Server=unused;Database=compatibility;Uid=unused;Pwd=unused;",
                new MySqlServerVersion(new Version(major, minor, patch)))
            .Options;
        using var context = new MySqlContext(options);

        Assert.IsFalse(context.Database.HasPendingModelChanges(),
            "The provider changed the model represented by the historical MySQL snapshot.");
    }
}
