using Aiursoft.CSTools.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.DbTools;

public static class ProgramExtends
{
    public static async Task<IHost> UpdateDbAsync<TContext>(
        this IHost host,
        CancellationToken token = default) where TContext : ICanMigrate
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
            "Updating database associated with context {ContextName}. In UT: {UT}",
            typeof(TContext).Name,
            EntryExtends.IsInUnitTests());

        try
        {
            await context.MigrateAsync(token);
            logger.LogInformation("Migrated database associated with context {ContextName}", typeof(TContext).Name);
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

    private static async Task WaitUntilCanConnect(ICanMigrate context, ILogger logger)
    {
            for (var i = 0;; i++)
            {
                if (i > 10)
                {
                    throw new InvalidOperationException("Cannot connect to database after 10 retries.");
                }
                try
                {
                    var canConnect = await context.CanConnectAsync();
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
