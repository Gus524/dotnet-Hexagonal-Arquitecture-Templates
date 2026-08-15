namespace SharedKernel.Repository;

public interface IQueryExecutor
{
    Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null, CancellationToken cancellationToken = default);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default);
}