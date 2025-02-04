using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementation
{
    public class DAOUtenti : DAOBase<Utente>, IDAOUtenti
    {
        public DAOUtenti(string connectionString) : base(connectionString)
        {
        }

        public void Delete(int id)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("DELETE FROM Utenti WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool Exists(int id)
        {
            return Exists("SELECT 1 FROM Utenti WHERE ID = @ID",
                new[] { new SqlParameter("@ID", id) });
        }

        public List<Utente> GetAll()
        {
            var utenti = new List<Utente>();
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione FROM Utenti", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                utenti.Add(new Utente
                {
                    ID = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Cognome = reader.GetString(2),
                    Email = reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    Ruolo = reader.GetString(5),
                    Team = reader.IsDBNull(6) ? null : reader.GetString(6),
                    DataCreazione = reader.GetDateTime(7)
                });
            }
            return utenti;
        }


        public Utente GetById(int id)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione FROM Utenti WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Utente
                {
                    ID = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Cognome = reader.GetString(2),
                    Email = reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    Ruolo = reader.GetString(5),
                    Team = reader.IsDBNull(6) ? null : reader.GetString(6),
                    DataCreazione = reader.GetDateTime(7)
                };
            }
            return null;
        }

        public void Insert(Utente entity)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand(
                    "INSERT INTO Utenti (Nome, Cognome, Email, PasswordHash, Ruolo, Team) " +
            "OUTPUT INSERTED.ID " +
            "VALUES (@Nome, @Cognome, @Email, @PasswordHash, @Ruolo, @Team)",
            conn, transaction);

                cmd.Parameters.AddWithValue("@Nome", entity.Nome);
                cmd.Parameters.AddWithValue("@Cognome", entity.Cognome);
                cmd.Parameters.AddWithValue("@Email", entity.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", $"HASHBYTES('SHA2_512','{entity.PasswordHash}')");
                cmd.Parameters.AddWithValue("@Ruolo", entity.Ruolo);
                cmd.Parameters.AddWithValue("@Team", (object)entity.Team ?? DBNull.Value);
                
                var newId = (int)cmd.ExecuteScalar();
                entity.ID = newId;

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Update(Utente entity)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(@"
        UPDATE Utenti
        SET Nome = @Nome,
            Cognome = @Cognome,
            Email = @Email,
            PasswordHash = @PasswordHash,
            Ruolo = @Ruolo,
            Team = @Team
        WHERE ID = @ID", conn);

            cmd.Parameters.AddWithValue("@ID", entity.ID);
            cmd.Parameters.AddWithValue("@Nome", entity.Nome);
            cmd.Parameters.AddWithValue("@Cognome", entity.Cognome);
            cmd.Parameters.AddWithValue("@Email", entity.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", $"HASHBYTES('SHA2_512','{entity.PasswordHash}')");
            cmd.Parameters.AddWithValue("@Ruolo", entity.Ruolo);
            cmd.Parameters.AddWithValue("@Team", (object)entity.Team ?? DBNull.Value);
           
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        public async Task<Utente> GetByEmail(string email)
{
    using var conn = CreateConnection();
    using var cmd = new SqlCommand(
        "SELECT ID, Nome, Cognome, Email, PasswordHash, Ruolo, Team, DataCreazione " +
        "FROM Utenti WHERE Email = @Email", conn);
    cmd.Parameters.AddWithValue("@Email", email);
    
    await conn.OpenAsync();
    using var reader = await cmd.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        return new Utente
        {
            ID = reader.GetInt32(0),
            Nome = reader.GetString(1),
            Cognome = reader.GetString(2),
            Email = reader.GetString(3),
            PasswordHash = reader.GetString(4),
            Ruolo = reader.GetString(5),
            Team = reader.IsDBNull(6) ? null : reader.GetString(6),
            DataCreazione = reader.GetDateTime(7)
        };
    }
    return null;
}
    }
