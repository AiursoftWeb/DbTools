using Aiursoft.CSTools.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.DbTools;

public enum UpdateMode
{
    /// <summary>
    /// Create the database and use it. Your project doesn't support migration. The db Context is fixed or requires manual SQL to update it. 
    /// </summary>
    CreateThenUse,

    /// <summary>
    /// Migrate the database and use it. If create directly, your database migration table is empty. So run all migrations instead. Make sure you have the 'Migrations' folder.
    /// </summary>
    MigrateThenUse,

    /// <summary>
    /// Drop everything and create everything. This is dangerous, but might be useful in UT.
    /// </summary>
    RecreateThenUse,
}

public static class ProgramExtends
{
    public static async Task<IHost> UpdateDbAsync<TContext>(
        this IHost host,
        UpdateMode mode) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        if (EntryExtends.IsInEntityFramework())
        {
            logger.LogWarning($"This programme was triggered by Entity framework. We should do nothing");
            return host;
        }

        var context = await services.GetRequiredServiceWithRetry<TContext>();
        
        await WaitUntilCanConnect(context, logger);
        
        logger.LogInformation(
            "Updating database associated with context {ContextName}. Is relational {Relational}. In UT: {UT}",
            typeof(TContext).Name,
            context.Database.IsRelational(),
            EntryExtends.IsInUnitTests());

        try
        {
            switch (mode)
            {
                case UpdateMode.CreateThenUse:
                    await context.Database.EnsureCreatedAsync();
                    break;
                case UpdateMode.MigrateThenUse:
                    await context.Database.MigrateAsync();
                    break;
                case UpdateMode.RecreateThenUse:
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                    break;
                default:
                    throw new InvalidOperationException($"Unknown mode: {mode}");
            }

            logger.LogInformation("{Mode} database associated with context {ContextName}", mode, typeof(TContext).Name);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Update database with context {ContextName} failed", typeof(TContext).Name);
            throw;
        }

        return host;
    }

    private static async Task<T> GetRequiredServiceWithRetry<T>(this IServiceProvider serviceProvider, int attempts = 5) where T : notnull
    {
        for (var i = 1; i <= attempts; i++)
        {
            try
            {
                var response = serviceProvider.GetRequiredService<T>();
                return response;
            }
            catch
            {
                if (i >= attempts)
                {
                    throw;
                }
                await Task.Delay(i * 1000);
            }
        }
        throw new InvalidOperationException("Code shall not reach here.");
    }

    private static async Task WaitUntilCanConnect(DbContext context, ILogger logger)
    {
        if (!(context.Database.ProviderName?.EndsWith("Sqlite", StringComparison.OrdinalIgnoreCase) ?? false))
        {
            for (var i = 0;; i++)
            {
                if (i > 10)
                {
                    throw new InvalidOperationException("Cannot connect to database after 10 retries.");
                }
                try
                {
                    var canConnect = await context.Database.CanConnectAsync();
                    if (canConnect)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Cannot connect to database. Retrying...");
                }
                
                await Task.Delay(i * 1000);
            }
        }
    }
}