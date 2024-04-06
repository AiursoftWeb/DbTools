using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.Sqlite;

public static class RegisterExtensions
{
    public static IServiceCollection AddAiurSqliteWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache = true,
        bool splitQuery = true)
        where T : DbContext
    {
        services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder
                .UseSqlite(
                    connectionString: connectionString,
                    sqliteOptionsAction: options =>
                    {
                        if (splitQuery)
                        {
                            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        }
                        // options.EnableRetryOnFailure();
                        options.CommandTimeout(30);
                        options.MigrationsAssembly(typeof(T).Assembly.FullName);
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
        });

        services.AddEFSecondLevelCache(options =>
        {
            if (allowCache)
            {
                options.UseMemoryCacheProvider().ConfigureLogging(enable: false);
                options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
            }
        });

        return services;
    }
}
