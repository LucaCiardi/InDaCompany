using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public class DAOLikes(string connectionString) : DAOBase<Like>(connectionString), IDAOLikes
    {
        public async Task<List<Like>> GetAllAsync()
        {
            const string query = "SELECT ID, UtenteID, ThreadID, MiPiace FROM Likes";
            var likes = new List<Like>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    likes.Add(MapFromReader(reader));
                }
                return likes;
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error retrieving all likes", ex);
            }
        }

        public async Task<Like?> GetByIdAsync(int id)
        {
            const string query = "SELECT ID, UtenteID, ThreadID, MiPiace FROM Likes WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExecuteQuerySingleAsync(query, parameters);
        }

        public async Task<int> InsertAsync(Like like)
        {
            const string query = @"
                INSERT INTO Likes (UtenteID, ThreadID, MiPiace) 
                VALUES (@UtenteID, @ThreadID, @MiPiace);
                SELECT SCOPE_IDENTITY();";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UtenteID", like.UtenteID);
            cmd.Parameters.AddWithValue("@ThreadID", like.ThreadID);
            cmd.Parameters.AddWithValue("@MiPiace", like.MiPiace);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error inserting like", ex);
            }
        }

        public async Task UpdateAsync(Like like)
        {
            const string query = @"
                UPDATE Likes 
                SET UtenteID = @UtenteID,
                    ThreadID = @ThreadID,
                    MiPiace = @MiPiace
                WHERE ID = @ID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ID", like.ID);
            cmd.Parameters.AddWithValue("@UtenteID", like.UtenteID);
            cmd.Parameters.AddWithValue("@ThreadID", like.ThreadID);
            cmd.Parameters.AddWithValue("@MiPiace", like.MiPiace);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error updating like {like.ID}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM Likes WHERE ID = @ID";

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
                throw new DAOException($"Error deleting like {id}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string query = "SELECT 1 FROM Likes WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExistsAsync(query, parameters);
        }

        public async Task<int> GetLikeCountAsync(int threadID)
        {
            const string query = "SELECT COUNT(*) FROM Likes WHERE ThreadID = @ThreadID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ThreadID", threadID);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error counting likes for thread {threadID}", ex);
            }
        }

        public async Task<bool> HasUserLikedPostAsync(int utenteID, int threadID)
        {
            const string query = "SELECT 1 FROM Likes WHERE UtenteID = @UtenteId AND ThreadID = @ThreadID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UtenteId", utenteID);
            cmd.Parameters.AddWithValue("@ThreadID", threadID);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != null;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error checking like status for user {utenteID} and thread {threadID}", ex);
            }
        }

        public async Task<int> ToggleLikeAsync(int utenteID, int threadID)
        {
            const string query = @"
                IF EXISTS (SELECT 1 FROM Likes WHERE UtenteID = @UtenteId AND ThreadID = @ThreadID)
                BEGIN
                    DELETE FROM Likes WHERE UtenteID = @UtenteId AND ThreadID = @ThreadID;
                    SELECT 0;
                END
                ELSE
                BEGIN
                    INSERT INTO Likes (UtenteID, ThreadID, MiPiace) VALUES (@UtenteId, @ThreadID, 1);
                    SELECT 1;
                END";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UtenteId", utenteID);
            cmd.Parameters.AddWithValue("@ThreadID", threadID);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error toggling like for user {utenteID} and thread {threadID}", ex);
            }
        }

        public async Task DeleteByPostAndUserAsync(int utenteID, int threadID)
        {
            const string query = "DELETE FROM Likes WHERE UtenteID = @UtenteId AND ThreadID = @ThreadID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UtenteId", utenteID);
            cmd.Parameters.AddWithValue("@ThreadID", threadID);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error deleting like for user {utenteID} and thread {threadID}", ex);
            }
        }

        protected override Like MapFromReader(SqlDataReader reader)
        {
            return new Like
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                UtenteID = reader.GetInt32(reader.GetOrdinal("UtenteID")),
                ThreadID = reader.GetInt32(reader.GetOrdinal("ThreadID")),
                MiPiace = reader.GetBoolean(reader.GetOrdinal("MiPiace"))
            };
        }
    }
}
