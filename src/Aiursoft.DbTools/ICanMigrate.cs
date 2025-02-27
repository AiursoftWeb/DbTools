namespace Aiursoft.DbTools;

public interface ICanMigrate
{
    Task MigrateAsync(CancellationToken cancellationToken);

    Task<bool> CanConnectAsync();
}