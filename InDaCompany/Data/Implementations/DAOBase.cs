using InDaCompany.Data.Implementations;
using Microsoft.Data.SqlClient;

public abstract class DAOBase<T>(string connectionString)
{
    protected readonly string _connectionString = connectionString ??
        throw new ArgumentNullException(nameof(connectionString));

    protected SqlConnection CreateConnection() => new(_connectionString);

    protected async Task<bool> ExistsAsync(string query, SqlParameter[] parameters)
    {
        await using var conn = CreateConnection();
        await using var cmd = new SqlCommand(query, conn);
        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        try
        {
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Database error checking existence: {ex.Message}", ex);
        }
    }

    protected async Task<List<T>> ExecuteQueryListAsync(string query, SqlParameter[]? parameters = null)
    {
        var results = new List<T>();
        await using var conn = CreateConnection();
        await using var cmd = new SqlCommand(query, conn);
        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        try
        {
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapFromReader(reader));
            }
            return results;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Database error executing query: {ex.Message}", ex);
        }
    }

    protected async Task<T?> ExecuteQuerySingleAsync(string query, SqlParameter[] parameters)
    {
        await using var conn = CreateConnection();
        await using var cmd = new SqlCommand(query, conn);
        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters);
        }

        try
        {
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapFromReader(reader);
            }
            return default;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Database error executing query: {ex.Message}", ex);
        }
    }

    protected abstract T MapFromReader(SqlDataReader reader);
}
