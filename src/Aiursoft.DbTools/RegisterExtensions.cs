using Aiursoft.CSTools.Tools;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        return services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder
                .UseSqlServer(
                    connectionString: connectionString,
                    sqlServerOptionsAction: options =>
                    {
                        options.CommandTimeout(30);
                        options.EnableRetryOnFailure();
                    });

            if (allowCache)
            {
                // Configure the DbContext to use memory cache
                var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
                optionsBuilder.UseMemoryCache(memoryCache);
            }
        });
    }
    
    public static IServiceCollection AddSqliteDbContextWithCache<T>(
        this IServiceCollection services, 
        string connectionString,
        bool allowCache = true)
        where T : DbContext
    {
        return services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder
                .UseSqlite(
                    connectionString: connectionString,
                    sqliteOptionsAction: options =>
                    {
                        options.CommandTimeout(30);
                    });

            if (allowCache)
            {
                // Configure the DbContext to use memory cache
                var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
                optionsBuilder.UseMemoryCache(memoryCache);
            }
        });
    }

    // TODO: Benchmark the performance and find best practice.
    public static IServiceCollection AddDbContextWithCacheThirdParty<T>(this IServiceCollection services, string connectionString)
        where T : DbContext
    {
        if (EntryExtends.IsInUnitTests())
        {
            services.AddDbContext<T>((serviceProvider, optionsBuilder) =>
                optionsBuilder
                    .UseInMemoryDatabase("inmemory")
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        }
        // Consider some new EF technology like use memory cache.
        else
        {
            services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
                optionsBuilder
                    .UseSqlServer(connectionString)
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        }

        services.AddEFSecondLevelCache(options =>
        {
            options.UseMemoryCacheProvider().DisableLogging(true);
            options.CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30));
        });
        return services;
    }
}