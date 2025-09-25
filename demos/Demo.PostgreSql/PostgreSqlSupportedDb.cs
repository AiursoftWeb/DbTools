using Aiursoft.DbTools;
using Aiursoft.DbTools.PostgreSql;
using Demo.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.PostgreSql;

public class PostgreSqlSupportedDb(bool allowCache, bool splitQuery) : SupportedDatabaseType<DemoDbContext>
{
    public override string DbType => "PostgreSql";

    public override IServiceCollection RegisterFunction(IServiceCollection services, string connectionString)
    {
        return services.AddAiurPostgreSqlWithCache<PostgreSqlContext>(
            connectionString,
            splitQuery: splitQuery,
            allowCache: allowCache);
    }

    public override DemoDbContext ContextResolver(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<PostgreSqlContext>();
    }
}
