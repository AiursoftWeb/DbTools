using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Aiursoft.DbTools;

public class ConnectionOpenedInterceptor(Action<DbConnection> onConnectionOpen) : DbConnectionInterceptor
{
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        onConnectionOpen(connection);
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        onConnectionOpen(connection);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}