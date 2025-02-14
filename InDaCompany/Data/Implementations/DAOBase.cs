using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public abstract class DAOBase<T> where T : class
    {
        protected readonly string _connectionString;

        protected DAOBase(string connectionString)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
        }

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected async Task<bool> ExistsAsync(string query, SqlParameter[] parameters)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != null && !Convert.IsDBNull(result);
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Errore database durante la verifica: {ex.Message}", ex);
            }
        }

        protected async Task<T?> ExecuteQuerySingleAsync(string query, SqlParameter[] parameters)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapFromReader(reader);
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Errore database durante l'esecuzione della query: {ex.Message}", ex);
            }
        }

        protected async Task<List<T>> ExecuteQueryListAsync(string query, SqlParameter[] parameters)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                var results = new List<T>();
                while (await reader.ReadAsync())
                {
                    results.Add(MapFromReader(reader));
                }
                return results;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Errore database durante l'esecuzione della query: {ex.Message}", ex);
            }
        }

        protected abstract T MapFromReader(SqlDataReader reader);
    }
}
