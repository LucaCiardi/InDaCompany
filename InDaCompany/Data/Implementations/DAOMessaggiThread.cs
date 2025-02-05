using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public class DAOMessaggiThread(string connectionString) : DAOBase<MessaggioThread>(connectionString), IDAOMessaggiThread
    {
        public async Task<List<MessaggioThread>> GetAllAsync()
        {
            const string query = @"
                SELECT ID, ThreadID, AutoreID, Testo, DataCreazione 
                FROM MessaggiThread";

            var messaggi = new List<MessaggioThread>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    messaggi.Add(MapFromReader(reader));
                }
                return messaggi;
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error retrieving messages", ex);
            }
        }

        public async Task<MessaggioThread?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT ID, ThreadID, AutoreID, Testo, DataCreazione 
                FROM MessaggiThread 
                WHERE ID = @ID";

            var parameters = new[] { new SqlParameter("@ID", id) };
            return await ExecuteQuerySingleAsync(query, parameters);
        }

        public async Task<int> InsertAsync(MessaggioThread entity, int threadID, int autoreID)
        {
            const string query = @"
                INSERT INTO MessaggiThread (ThreadID, AutoreID, Testo) 
                VALUES (@ThreadID, @AutoreID, @Testo);
                SELECT SCOPE_IDENTITY();";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ThreadID", threadID);
            cmd.Parameters.AddWithValue("@AutoreID", autoreID);
            cmd.Parameters.AddWithValue("@Testo", entity.Testo);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error inserting message", ex);
            }
        }

        public async Task UpdateAsync(MessaggioThread entity)
        {
            const string query = @"
                UPDATE MessaggiThread
                SET ThreadID = @ThreadID, 
                    AutoreID = @AutoreID, 
                    Testo = @Testo
                WHERE ID = @ID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ID", entity.ID);
            cmd.Parameters.AddWithValue("@ThreadID", entity.ThreadID);
            cmd.Parameters.AddWithValue("@AutoreID", entity.AutoreID);
            cmd.Parameters.AddWithValue("@Testo", entity.Testo);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error updating message", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM MessaggiThread WHERE ID = @ID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ID", id);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error deleting message", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string query = "SELECT 1 FROM MessaggiThread WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExistsAsync(query, parameters);
        }

        protected override MessaggioThread MapFromReader(SqlDataReader reader)
        {
            return new MessaggioThread
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                ThreadID = reader.GetInt32(reader.GetOrdinal("ThreadID")),
                AutoreID = reader.GetInt32(reader.GetOrdinal("AutoreID")),
                Testo = reader.GetString(reader.GetOrdinal("Testo")),
                DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione"))
            };
        }
        public async Task<int> InsertAsync(MessaggioThread entity)
        {
            const string query = @"
                INSERT INTO MessaggiThread (ThreadID, AutoreID, Testo) 
                VALUES (@ThreadID, @AutoreID, @Testo);
                SELECT SCOPE_IDENTITY();";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ThreadID", entity.ThreadID);
            cmd.Parameters.AddWithValue("@AutoreID", entity.AutoreID);
            cmd.Parameters.AddWithValue("@Testo", entity.Testo);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error inserting message", ex);
            }
        }
    }
}