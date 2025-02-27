using Aiursoft.CSTools.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.Switchable;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwitchableRelationalDatabase<TAbstractContext>(
        this IServiceCollection services,
        string dbType,
        string connectionString,
        List<SupportedDatabaseType<TAbstractContext>> supportedDbs) where TAbstractContext : DbContext
    {
        var supportedDb = supportedDbs.SingleOrDefault(x => string.Equals(x.DbType, dbType, StringComparison.OrdinalIgnoreCase));
        if (supportedDb == null)
        {
            var supportedTypes = string.Join(", ", supportedDbs.SelectMany(t => t.DbType));
            throw new NotSupportedException(
                $"Database type {dbType} is not supported! Supported database types: {supportedTypes}");
        }

        Console.WriteLine($"Using database type: {supportedDb.DbType}. Connection string: {connectionString.SafeSubstring(20)}");
        var builtServices = supportedDb.RegisterFunction(services, connectionString);
        services.AddScoped<TAbstractContext>(t => supportedDb.ContextResolver(t));
        return builtServices;
    }

    public static (string connectionString, string dbType, bool allowCacheLayer) GetDbSettings(
        this IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        var dbType = configuration.GetSection("ConnectionStrings:DbType").Get<string>()!;
        var allowCache = configuration.GetSection("ConnectionStrings:AllowCache").Get<bool>();

        return (connectionString, dbType, allowCache);
    }
}
