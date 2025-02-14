using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public class DAOThreadForum(string connectionString) : DAOBase<ThreadForum>(connectionString), IDAOThreadForum
    {
        public async Task<List<ThreadForum>> GetAllAsync()
        {
            const string query = @"
                SELECT ID, Titolo, Testo, ForumID, AutoreID, DataCreazione 
                FROM ThreadForum";
            var threads = new List<ThreadForum>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    threads.Add(MapFromReader(reader));
                }
                return threads;
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error retrieving threads", ex);
            }
        }

        public async Task<List<ThreadForum>> GetHomeThreadsAsync()
        {
            const string query = @"
                SELECT ThreadForum.ID, ThreadForum.Titolo, ThreadForum.Testo, ThreadForum.ForumID, ThreadForum.AutoreID, ThreadForum.DataCreazione 
                FROM ThreadForum
                LEFT JOIN Forum ON ThreadForum.ForumID = Forum.ID
                WHERE Forum.Nome = 'Generale'";
            var threads = new List<ThreadForum>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    threads.Add(MapFromReader(reader));
                }
                return threads;
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error retrieving threads", ex);
            }
        }

        public async Task<ThreadForum?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT ID, Titolo, Testo, ForumID, AutoreID, DataCreazione 
                FROM ThreadForum 
                WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExecuteQuerySingleAsync(query, parameters);
        }

        public async Task<int> InsertAsync(ThreadForum entity)
        {
            return await InsertWithIdsAsync(entity, entity.ForumID, entity.AutoreID);
        }

        public async Task<int> InsertWithIdsAsync(ThreadForum entity, int forumID, int autoreID)
        {
            const string query = @"
                INSERT INTO ThreadForum (Titolo, Testo, ForumID, AutoreID) 
                VALUES (@Titolo, @ForumID, @AutoreID);
                SELECT SCOPE_IDENTITY();";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Titolo", entity.Titolo);
            cmd.Parameters.AddWithValue("@ForumID", forumID);
            cmd.Parameters.AddWithValue("@AutoreID", autoreID);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error inserting thread", ex);
            }
        }

        public async Task UpdateAsync(ThreadForum entity)
        {
            const string query = @"
                UPDATE ThreadForum
                SET Titolo = @Titolo,
                    Testo = @Testo,
                    ForumID = @ForumID, 
                    AutoreID = @AutoreID
                WHERE ID = @ID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ID", entity.ID);
            cmd.Parameters.AddWithValue("@Titolo", entity.Titolo);
            cmd.Parameters.AddWithValue("@Testo", entity.Testo);
            cmd.Parameters.AddWithValue("@ForumID", entity.ForumID);
            cmd.Parameters.AddWithValue("@AutoreID", entity.AutoreID);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error updating thread {entity.ID}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM ThreadForum WHERE ID = @ID";

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
                throw new DAOException($"Error deleting thread {id}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string query = "SELECT 1 FROM ThreadForum WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExistsAsync(query, parameters);
        }

        protected override ThreadForum MapFromReader(SqlDataReader reader)
        {
            return new ThreadForum
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                Titolo = reader.GetString(reader.GetOrdinal("Titolo")),
                Testo = reader.GetString(reader.GetOrdinal("Testo")),
                ForumID = reader.GetInt32(reader.GetOrdinal("ForumID")),
                AutoreID = reader.GetInt32(reader.GetOrdinal("AutoreID")),
                DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione"))
            };
        }
    }
}
