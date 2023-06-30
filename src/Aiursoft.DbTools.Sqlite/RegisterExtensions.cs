using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.Sqlite;

public static class RegisterExtensions
{
    public static IServiceCollection AddAiurSqliteWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache = true)
        where T : DbContext
    {
        services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder
                .UseSqlite(
                    connectionString: connectionString,
                    sqliteOptionsAction: options =>
                    {
                        options.CommandTimeout(30);
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
        });

        services.AddEFSecondLevelCache(options =>
        {
            if (allowCache)
            {
                options.UseMemoryCacheProvider().DisableLogging(true);
                options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
            }
        });

        return services;
    }
}
