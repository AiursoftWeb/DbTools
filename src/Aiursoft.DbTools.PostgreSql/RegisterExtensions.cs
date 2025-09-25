using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.PostgreSql;

public static class RegisterExtensions
{
    public static IServiceCollection AddAiurPostgreSqlWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache = true,
        bool splitQuery = true)
        where T : DbContext
    {
        services.AddDbContext<T>((serviceProvider, optionsAction) =>
        {
            var builder = optionsAction
                .UseNpgsql(connectionString,
                    options =>
                    {
                        if (splitQuery)
                        {
                            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        }

                        options.EnableRetryOnFailure();
                        options.CommandTimeout(30);
                        options.MigrationsAssembly(typeof(T).Assembly.FullName);
                    })
                .EnableDetailedErrors();

            if (allowCache)
            {
                builder.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
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