using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public class DAOPost(string connectionString) : DAOBase<Post>(connectionString), IDAOPost
    {
        public async Task<List<Post>> GetAllAsync()
        {
            const string query = "SELECT ID, Testo, DataCreazione, AutoreID FROM Post";
            var posts = new List<Post>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    posts.Add(MapFromReader(reader));
                }
                return posts;
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error retrieving posts", ex);
            }
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            const string query = "SELECT ID, Testo, DataCreazione, AutoreID FROM Post WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExecuteQuerySingleAsync(query, parameters);
        }

        public async Task<List<Post>> GetByAutoreIDAsync(int autoreID)
        {
            const string query = "SELECT ID, Testo, DataCreazione, AutoreID FROM Post WHERE AutoreID = @AutoreID";
            var posts = new List<Post>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@AutoreID", autoreID);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    posts.Add(MapFromReader(reader));
                }
                return posts;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error retrieving posts for author {autoreID}", ex);
            }
        }

        public async Task<List<Post>> GetByDataCreazioneAsync(DateTime dataCreazione)
        {
            const string query = "SELECT ID, Testo, DataCreazione, AutoreID FROM Post WHERE CAST(DataCreazione AS DATE) = @DataCreazione";
            var posts = new List<Post>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@DataCreazione", dataCreazione.Date);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    posts.Add(MapFromReader(reader));
                }
                return posts;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error retrieving posts for date {dataCreazione:d}", ex);
            }
        }

        public async Task<List<Post>> SearchAsync(string searchTerm)
        {
            const string query = "SELECT ID, Testo, DataCreazione, AutoreID FROM Post WHERE Testo LIKE @SearchTerm";
            var posts = new List<Post>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    posts.Add(MapFromReader(reader));
                }
                return posts;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error searching posts with term '{searchTerm}'", ex);
            }
        }

        public async Task<int> InsertAsync(Post post)
        {
            const string query = @"
                INSERT INTO Post (Testo, AutoreID) 
                VALUES (@Testo, @AutoreID);
                SELECT SCOPE_IDENTITY();";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Testo", post.Testo);
            cmd.Parameters.AddWithValue("@AutoreID", post.AutoreID);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error inserting post", ex);
            }
        }

        public async Task UpdateAsync(Post post)
        {
            const string query = @"
                UPDATE Post 
                SET Testo = @Testo, 
                    AutoreID = @AutoreID 
                WHERE ID = @ID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ID", post.ID);
            cmd.Parameters.AddWithValue("@Testo", post.Testo);
            cmd.Parameters.AddWithValue("@AutoreID", post.AutoreID);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error updating post {post.ID}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM Post WHERE ID = @ID";

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
                throw new DAOException($"Error deleting post {id}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string query = "SELECT 1 FROM Post WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExistsAsync(query, parameters);
        }

        protected override Post MapFromReader(SqlDataReader reader)
        {
            return new Post
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                Testo = reader.GetString(reader.GetOrdinal("Testo")),
                DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione")),
                AutoreID = reader.GetInt32(reader.GetOrdinal("AutoreID"))
            };
        }
    }
}
