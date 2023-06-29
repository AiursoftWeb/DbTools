using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.DbTools.Tests
{
    [TestClass]
    public class ProgramExtendsTests
    {
        [TestMethod]
        public async Task UpdateDbAsync_ShouldNotMigrate_WhenInEntityFramework()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder();
            hostBuilder.ConfigureServices(services => 
                services.AddAiurSqliteWithCache<MyDbContext>(@"DataSource=app.db;Cache=Shared")
            );
            var host = hostBuilder.Build();
            _ = await host.UpdateDbAsync<MyDbContext>(UpdateMode.RecreateThenUse);
        }
    }
}