using Aiursoft.CSTools.Tools;
using Aiursoft.DbTools.InMemory;
using Aiursoft.DbTools.MySql;
using Aiursoft.DbTools.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.DbTools.Switchable;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase<Context>(this IServiceCollection services, 
        string connectionString, 
        DbType dbType,
        bool allowCache) where Context : DbContext
    {
        if (EntryExtends.IsInUnitTests())
        {
            Console.WriteLine("Unit test detected, using in-memory database.");
            dbType = DbType.InMemory;
        }
        
        switch (dbType)
        {
            case DbType.InMemory:
                services.AddAiurInMemoryDb<Context>();
                break;
            case DbType.Sqlite:
                services.AddAiurSqliteWithCache<Context>(connectionString, allowCache);
                break;
            case DbType.MySql:
                services.AddAiurMySqlWithCache<Context>(connectionString, allowCache);
                break;
            default:
                throw new NotSupportedException($"Database type {dbType} is not supported!");
        }

        return services;
    }
}