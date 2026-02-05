using System.Data.Common;
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
        return services.AddAiurSqliteWithCache<T>(connectionString, allowCache, splitQuery, null);
    }

    public static IServiceCollection AddAiurSqliteWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache,
        bool splitQuery,
        Action<DbConnection>? onConnectionOpen)
        where T : DbContext
    {
        services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
        {
            var builder = optionsBuilder
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
                    });

            if (allowCache)
            {
                builder.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
            }

            if (onConnectionOpen != null)
            {
                builder.AddInterceptors(new ConnectionOpenedInterceptor(onConnectionOpen));
            }
        });

        if (allowCache)
        {
            services.AddEFSecondLevelCache(options =>
            {
                options.UseMemoryCacheProvider().ConfigureLogging(enable: false);
                options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                options.UseDbCallsIfCachingProviderIsDown(TimeSpan.FromMinutes(1));
            });
        }

        return services;
    }
}