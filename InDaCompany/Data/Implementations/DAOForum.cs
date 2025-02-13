using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public class DAOForum(string connectionString) : DAOBase<Forum>(connectionString), IDAOForum
    {
        public async Task<List<Forum>> GetAllAsync()
        {
            const string query = "SELECT ID, Nome, Descrizione, Team FROM Forum";
            var forums = new List<Forum>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    forums.Add(MapFromReader(reader));
                }
                return forums;
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error retrieving forums", ex);
            }
        }

        public async Task<Forum?> GetByIdAsync(int id)
        {
            const string query = "SELECT ID, Nome, Descrizione, Team FROM Forum WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExecuteQuerySingleAsync(query, parameters);
        }

        public async Task<List<Forum>> GetForumByUser(string teamUser) {

            const string query = @"
                SELECT ID, Nome, Descrizione, Team 
                FROM Forum 
                WHERE Team = @TeamUser";
            var forums = new List<Forum>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TeamUser", teamUser);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    forums.Add(MapFromReader(reader));
                }
                return forums;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error retrieving tickets for creator {teamUser}", ex);
            }
        }

        public async Task<int> InsertAsync(Forum forum)
        {
            const string query = @"
                INSERT INTO Forum (Nome, Descrizione, Team) 
                VALUES (@Nome, @Descrizione, @Team);
                SELECT SCOPE_IDENTITY();";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Nome", forum.Nome);
            cmd.Parameters.AddWithValue("@Descrizione", (object?)forum.Descrizione ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Team", (object?)forum.Team ?? DBNull.Value);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error inserting forum", ex);
            }
        }

        public async Task UpdateAsync(Forum forum)
        {
            const string query = @"
                UPDATE Forum 
                SET Nome = @Nome, 
                    Descrizione = @Descrizione, 
                    Team = @Team 
                WHERE ID = @ID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ID", forum.ID);
            cmd.Parameters.AddWithValue("@Nome", forum.Nome);
            cmd.Parameters.AddWithValue("@Descrizione", (object?)forum.Descrizione ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Team", (object?)forum.Team ?? DBNull.Value);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error updating forum", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM Forum WHERE ID = @ID";

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
                throw new DAOException("Error deleting forum", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string query = "SELECT 1 FROM Forum WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExistsAsync(query, parameters);
        }

        protected override Forum MapFromReader(SqlDataReader reader)
        {
            return new Forum
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                Nome = reader.GetString(reader.GetOrdinal("Nome")),
                Descrizione = reader.IsDBNull(reader.GetOrdinal("Descrizione"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Descrizione")),
                Team = reader.IsDBNull(reader.GetOrdinal("Team"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Team"))
            };
        }
    }
}