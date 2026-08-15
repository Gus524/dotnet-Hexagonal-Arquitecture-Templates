using Dapper;
using Microsoft.Data.SqlClient;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Repository;

namespace Common.QueryExecutor;

public class QueryExecutor(IConnectionResolver connectionResolver) : IQueryExecutor
{
    private SqlConnection CreateConnection() => new(connectionResolver.GetConnectionString());
    public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null, CancellationToken cancellationToken = default)
    {
        await using var connection = CreateConnection();
        return await connection.QueryAsync<T>(new CommandDefinition(query, parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, param, cancellationToken: ct));
    }
}