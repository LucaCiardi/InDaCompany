using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public abstract class DAOBase<T>(string connectionString)
    {
        protected readonly string _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected async Task<bool> ExistsAsync(string query, SqlParameter[] parameters)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
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

        protected async Task<T?> ExecuteQuerySingleAsync(string query, SqlParameter[] parameters)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
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
}

