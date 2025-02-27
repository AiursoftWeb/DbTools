using System.Diagnostics.CodeAnalysis;
using static Aiursoft.WebTools.Extends;
using Aiursoft.DbTools;
using Demo.Entities;

namespace Demo.WebApp;

[ExcludeFromCodeCoverage]
public abstract class Program
{
    public static async Task Main(string[] args)
    {
        var app = await AppAsync<Startup>(args);

        // Before starting, make sure all migration files are generated:

        // cd ./Demo.MySql
        // dotnet ef migrations add Init --context "MySqlContext" -s ../Demo.WebApp/Demo.WebApp.csproj
        // cd ..

        // cd ./Demo.Sqlite
        // dotnet ef migrations add Init --context "SqliteContext" -s ../Demo.WebApp/Demo.WebApp.csproj
        // cd ..
        await app.UpdateDbAsync<DemoDbContext>();
        await app.RunAsync();
    }
}
