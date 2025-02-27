using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools;

public abstract class SupportedDatabaseType<TAbstractContext> where TAbstractContext : DbContext
{
    public abstract string DbType { get; }

    public abstract IServiceCollection RegisterFunction(IServiceCollection services, string connectionString);

    public abstract TAbstractContext ContextResolver(IServiceProvider serviceProvider);
}
