using InDaCompany.Data.Interfaces;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementation
{
    public abstract class DAOBase<T>
    {
        protected readonly string _connectionString;

        protected DAOBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected bool Exists(string query, SqlParameter[] parameters)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return result != null;
        }
    }
}
