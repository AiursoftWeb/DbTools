using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.SqlServer;

public static class RegisterExtensions
{
    public static IServiceCollection AddAiurSqlServerWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache = true,
        bool splitQuery = true)
        where T : DbContext
    {
        services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
            optionsBuilder
                .UseSqlServer(
                    connectionString: connectionString,
                    sqlServerOptionsAction: options =>
                    {
                        if (splitQuery)
                        {
                            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        }

                        options.EnableRetryOnFailure();
                        options.CommandTimeout(30);
                        options.MigrationsAssembly(typeof(T).Assembly.FullName);
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));

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