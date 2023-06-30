using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.InMemory;

public static class RegisterExtensions
{
    public static IServiceCollection AddAiurMySqlWithCache<T>(
        this IServiceCollection services,
        string connectionString,
        bool allowCache = true)
        where T : DbContext
    {
        services.AddDbContext<T>((serviceProvider, optionsAction) => optionsAction
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                builder =>
                {
                    builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
                })
            .EnableDetailedErrors()
            .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        
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