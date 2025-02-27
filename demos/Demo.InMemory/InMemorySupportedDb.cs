using Aiursoft.DbTools;
using Aiursoft.DbTools.InMemory;
using Demo.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.InMemory;

public class InMemorySupportedDb : SupportedDatabaseType<DemoDbContext>
{
    public override string DbType => "InMemory";

    public override IServiceCollection RegisterFunction(IServiceCollection services, string connectionString)
    {
        return services.AddAiurInMemoryDb<InMemoryContext>();
    }

    public override DemoDbContext ContextResolver(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<InMemoryContext>();
    }
}