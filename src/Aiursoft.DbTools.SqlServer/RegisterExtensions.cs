﻿using EFCoreSecondLevelCacheInterceptor;
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
                    })
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
