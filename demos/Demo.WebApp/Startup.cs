using Aiursoft.CSTools.Tools;
using Aiursoft.DbTools.Switchable;
using Aiursoft.WebTools.Abstractions.Models;
using Demo.InMemory;
using Demo.MySql;
using Demo.Sqlite;
using Demo.PostgreSql;

namespace Demo.WebApp;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        var (connectionString, dbType, allowCache) = configuration.GetDbSettings();
        services.AddSwitchableRelationalDatabase(
            dbType: EntryExtends.IsInUnitTests() ? "InMemory": dbType,
            connectionString: connectionString,
            supportedDbs:
            [
                new MySqlSupportedDb(allowCache: allowCache, splitQuery: false),
                new PostgreSqlSupportedDb(allowCache: allowCache, splitQuery: false),
                new SqliteSupportedDb(allowCache: allowCache, splitQuery: true),
                new InMemorySupportedDb()
            ]);

        services
            .AddControllersWithViews()
            .AddApplicationPart(typeof(Startup).Assembly);
    }

    public void Configure(WebApplication app)
    {
        app.UseRouting();
        app.MapDefaultControllerRoute();
    }
}
