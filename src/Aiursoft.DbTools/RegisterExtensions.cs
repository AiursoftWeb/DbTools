using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools;

public static class RegisterExtensions
{
    public static IServiceCollection AddSqlServerDbContextWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache = true)
        where T : DbContext
    {
        services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
            optionsBuilder
                .UseSqlServer(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));

        services.AddEFSecondLevelCache(options =>
        {
            options.UseMemoryCacheProvider().DisableLogging(true);
            options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
        });
        return services;
    }

    public static IServiceCollection AddSqliteDbContext<T>(
        this IServiceCollection services,
        string connectionString)
        where T : DbContext
    {
        return services.AddDbContext<T>(optionsBuilder =>
        {
            optionsBuilder
                .UseSqlite(
                    connectionString: connectionString,
                    sqliteOptionsAction: options => { options.CommandTimeout(30); });
        });
    }

    public static IServiceCollection AddInMemoryDb<T>(
        this IServiceCollection services)
        where T : DbContext
    {
        return services.AddDbContext<T>(optionsBuilder =>
        {
            optionsBuilder
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        });
    }
}