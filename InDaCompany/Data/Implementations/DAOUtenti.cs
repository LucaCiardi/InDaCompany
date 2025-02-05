using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;


namespace InDaCompany.Data.Implementations
{
    public class DAOUtenti(string connectionString) : DAOBase<Utente>(connectionString), IDAOUtenti
    {
        public async Task<List<Utente>> GetAllAsync()
        {
            const string query = @"
                SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione 
                FROM Utenti";
            var utenti = new List<Utente>();

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    utenti.Add(MapFromReader(reader));
                }
                return utenti;
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error retrieving users", ex);
            }
        }

        public async Task<Utente?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione 
                FROM Utenti 
                WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExecuteQuerySingleAsync(query, parameters);
        }

        public async Task<int> InsertAsync(Utente entity)
        {
            const string query = @"
                INSERT INTO Utenti (Nome, Cognome, Email, PasswordHash, Ruolo, Team) 
                VALUES (@Nome, @Cognome, @Email, @PasswordHash, @Ruolo, @Team);
                SELECT SCOPE_IDENTITY();";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Nome", entity.Nome);
            cmd.Parameters.AddWithValue("@Cognome", entity.Cognome);
            cmd.Parameters.AddWithValue("@Email", entity.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
            cmd.Parameters.AddWithValue("@Ruolo", entity.Ruolo);
            cmd.Parameters.AddWithValue("@Team", (object?)entity.Team ?? DBNull.Value);

            try
            {
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Error inserting user", ex);
            }
        }

        public async Task UpdateAsync(Utente entity)
        {
            const string query = @"
                UPDATE Utenti 
                SET Nome = @Nome,
                    Cognome = @Cognome,
                    Email = @Email,
                    PasswordHash = @PasswordHash,
                    Ruolo = @Ruolo,
                    Team = @Team
                WHERE ID = @ID";

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ID", entity.ID);
            cmd.Parameters.AddWithValue("@Nome", entity.Nome);
            cmd.Parameters.AddWithValue("@Cognome", entity.Cognome);
            cmd.Parameters.AddWithValue("@Email", entity.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
            cmd.Parameters.AddWithValue("@Ruolo", entity.Ruolo);
            cmd.Parameters.AddWithValue("@Team", (object?)entity.Team ?? DBNull.Value);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Error updating user {entity.ID}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM Utenti WHERE ID = @ID";

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
                throw new DAOException($"Error deleting user {id}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string query = "SELECT 1 FROM Utenti WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };

            return await ExistsAsync(query, parameters);
        }

        protected override Utente MapFromReader(SqlDataReader reader)
        {
            return new Utente
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                Nome = reader.GetString(reader.GetOrdinal("Nome")),
                Cognome = reader.GetString(reader.GetOrdinal("Cognome")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                Ruolo = reader.GetString(reader.GetOrdinal("Ruolo")),
                Team = reader.IsDBNull(reader.GetOrdinal("Team"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Team")),
                DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione"))
            };
        }
    }
}
