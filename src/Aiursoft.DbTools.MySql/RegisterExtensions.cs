using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.MySql;

public static class RegisterExtensions
{
    public static IServiceCollection AddAiurMySqlWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache = true,
        bool splitQuery = true)
        where T : DbContext
    {
        services.AddDbContext<T>((serviceProvider, optionsAction) => optionsAction
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
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
            .EnableDetailedErrors()
            .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        
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