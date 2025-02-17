using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations
{
    public class DAOUtenti : DAOBase<Utente>, IDAOUtenti
    {
        public DAOUtenti(string connectionString) : base(connectionString) { }

        public async Task<List<Utente>> GetAllAsync()
        {
            const string query = @"
            SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione, FotoProfilo 
            FROM Utenti";

            return await ExecuteQueryListAsync(query, Array.Empty<SqlParameter>());
        }

        public async Task<Utente?> GetByIdAsync(int id)
        {
            const string query = @"
            SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione, FotoProfilo 
            FROM Utenti 
            WHERE ID = @ID";

            var parameters = new[] { new SqlParameter("@ID", id) };
            return await ExecuteQuerySingleAsync(query, parameters);
        }


        public async Task<Utente?> GetByEmailAsync(string email)
        {
            const string query = @"
            SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione, FotoProfilo 
            FROM Utenti 
            WHERE Email = @Email";

            var parameters = new[] { new SqlParameter("@Email", email) };
            return await ExecuteQuerySingleAsync(query, parameters);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            const string query = "SELECT 1 FROM Utenti WHERE Email = @Email";
            var parameters = new[] { new SqlParameter("@Email", email) };
            var exists = await ExistsAsync(query, parameters);
            return !exists;
        }
        public async Task<List<Utente>> GetByTeamAsync(string team)
        {
            const string query = @"
        SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, FotoProfilo, DataCreazione
        FROM Utenti 
        WHERE Team = @Team
        ORDER BY Cognome, Nome";

            var parameters = new[] { new SqlParameter("@Team", team) };
            return await ExecuteQueryListAsync(query, parameters);
        }

        public async Task<int> InsertAsync(Utente entity)
        {
            const string query = @"
            INSERT INTO Utenti (Nome, Cognome, Email, PasswordHash, Ruolo, Team,  FotoProfilo) 
            VALUES (@Nome, @Cognome, @Email, @PasswordHash, @Ruolo, @Team,  @FotoProfilo);
            SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
            new SqlParameter("@Nome", entity.Nome),
            new SqlParameter("@Cognome", entity.Cognome),
            new SqlParameter("@Email", entity.Email),
            new SqlParameter("@PasswordHash", entity.PasswordHash),
            new SqlParameter("@Ruolo", entity.Ruolo),
            new SqlParameter("@Team", (object?)entity.Team ?? DBNull.Value),
            new SqlParameter("@FotoProfilo", (object?)entity.FotoProfilo ?? DBNull.Value)
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
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new DAOException("Email già registrata nel sistema", ex);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Errore durante l'inserimento dell'utente", ex);
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
                Team = @Team,
                FotoProfilo = @FotoProfilo
            WHERE ID = @ID";

            var parameters = new[]
            {
            new SqlParameter("@ID", entity.ID),
            new SqlParameter("@Nome", entity.Nome),
            new SqlParameter("@Cognome", entity.Cognome),
            new SqlParameter("@Email", entity.Email),
            new SqlParameter("@PasswordHash", entity.PasswordHash),
            new SqlParameter("@Ruolo", entity.Ruolo),
            new SqlParameter("@Team", (object?)entity.Team ?? DBNull.Value),
            new SqlParameter("@FotoProfilo", (object?)entity.FotoProfilo ?? DBNull.Value)
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
                    throw new DAOException($"Nessun utente trovato con ID {entity.ID}");
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new DAOException("Email già registrata nel sistema", ex);
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Errore durante l'aggiornamento dell'utente {entity.ID}", ex);
            }
        }
        public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash)
        {
            const string query = @"
                UPDATE Utenti 
                SET PasswordHash = @PasswordHash
                WHERE ID = @ID";

            var parameters = new[]
            {
                new SqlParameter("@ID", userId),
                new SqlParameter("@PasswordHash", newPasswordHash)
            };

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Errore durante il cambio password dell'utente {userId}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM Utenti WHERE ID = @ID";
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
                    throw new DAOException($"Nessun utente trovato con ID {id}");
                }
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Errore durante l'eliminazione dell'utente {id}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string query = "SELECT 1 FROM Utenti WHERE ID = @ID";
            var parameters = new[] { new SqlParameter("@ID", id) };
            return await ExistsAsync(query, parameters);
        }

        public async Task<Utente?> AuthenticateAsync(string email, string password)
        {
            const string query = @"
    SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione, FotoProfilo
    FROM Utenti 
    WHERE Email = @Email AND PasswordHash = @Password";


            var parameters = new[]
            {
                new SqlParameter("@Email", email),
                new SqlParameter("@Password", password)
            };

            try
            {
                return await ExecuteQuerySingleAsync(query, parameters);
            }
            catch (SqlException ex)
            {
                throw new DAOException("Errore durante l'autenticazione", ex);
            }
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
                DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione")),
                FotoProfilo = reader.IsDBNull(reader.GetOrdinal("FotoProfilo"))
            ? null
            : (byte[])reader.GetValue(reader.GetOrdinal("FotoProfilo"))
            };
        }
        public async Task UpdateProfilePictureAsync(int userId, byte[] imageData)
        {
            const string query = "UPDATE Utenti SET FotoProfilo = @FotoProfilo WHERE ID = @ID";
            var parameters = new[]
            {
        new SqlParameter("@ID", userId),
        new SqlParameter("@FotoProfilo", (object?)imageData ?? DBNull.Value)
    };

            using var conn = CreateConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DAOException($"Errore durante l'aggiornamento della foto profilo per l'utente {userId}", ex);
            }
        }

        public async Task SetDefaultProfilePictureAsync(int userId)
        {
            var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "images", "profile.jpg");
            byte[] defaultImageBytes = await File.ReadAllBytesAsync(defaultAvatarPath);

            await UpdateProfilePictureAsync(userId, defaultImageBytes);
        }

    }
}
