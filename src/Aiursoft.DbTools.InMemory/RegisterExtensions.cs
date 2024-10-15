using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.InMemory;

public static class RegisterExtensions
{
    public static IServiceCollection AddAiurInMemoryDb<T>(
        this IServiceCollection services)
        where T : DbContext
    {
        return services.AddDbContext<T>(optionsBuilder =>
        {
            optionsBuilder
                .UseInMemoryDatabase(databaseName: "in-memory")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
    }
}
