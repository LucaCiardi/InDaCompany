using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public class DAOMessaggiThread : DAOBase<MessaggioThread>, IDAOMessaggiThread
    {
        public DAOMessaggiThread(string connectionString) : base(connectionString) { }

        public async Task<List<MessaggioThread>> GetAllAsync()
        {
            const string query = @"
                SELECT ID, ThreadID, AutoreID, Testo, DataCreazione 
                FROM MessaggiThread
                ORDER BY DataCreazione DESC";

            return await ExecuteQueryListAsync(query, Array.Empty<SqlParameter>());
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

        public async Task<List<MessaggioThread>> GetMessagesByThreadAsync(int threadID)
        {
            const string query = @"
                SELECT ID, ThreadID, AutoreID, Testo, DataCreazione 
                FROM MessaggiThread 
                WHERE ThreadID = @ThreadID
                ORDER BY DataCreazione ASC";

            var parameters = new[] { new SqlParameter("@ThreadID", threadID) };
            return await ExecuteQueryListAsync(query, parameters);
        }

        public async Task<List<MessaggioThread>> GetMessagesByAuthorAsync(int authorID)
        {
            const string query = @"
                SELECT ID, ThreadID, AutoreID, Testo, DataCreazione 
                FROM MessaggiThread 
                WHERE AutoreID = @AutoreID
                ORDER BY DataCreazione DESC";

            var parameters = new[] { new SqlParameter("@AutoreID", authorID) };
            return await ExecuteQueryListAsync(query, parameters);
        }
        public async Task<int> InsertAsync(MessaggioThread entity)
        {
            const string query = @"
                INSERT INTO MessaggiThread (ThreadID, AutoreID, Testo, DataCreazione) 
                VALUES (@ThreadID, @AutoreID, @Testo, @DataCreazione);
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                new SqlParameter("@ThreadID", entity.ThreadID),
                new SqlParameter("@AutoreID", entity.AutoreID),
                new SqlParameter("@Testo", entity.Testo),
                new SqlParameter("@DataCreazione", DateTime.Now)
            };

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Errore durante l'inserimento del messaggio", ex);
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

            var parameters = new[]
            {
                new SqlParameter("@ID", entity.ID),
                new SqlParameter("@ThreadID", entity.ThreadID),
                new SqlParameter("@AutoreID", entity.AutoreID),
                new SqlParameter("@Testo", entity.Testo)
            };

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    throw new DAOException($"Nessun messaggio trovato con ID {entity.ID}");
                }
            }
            catch (SqlException ex)
            {
                throw new DAOException("Errore durante l'aggiornamento del messaggio", ex);
            }
        }
        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM MessaggiThread WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    throw new DAOException($"Nessun messaggio trovato con ID {id}");
                }
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Errore durante l'eliminazione del messaggio {id}", ex);
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
    }
}
