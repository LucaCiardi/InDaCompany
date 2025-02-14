using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

public class DAOTicket : DAOBase<Ticket>, IDAOTicket
{
    public DAOTicket(string connectionString) : base(connectionString) { }

    public async Task<List<Ticket>> GetAllAsync()
    {
        const string query = @"
            SELECT ID, Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket
            ORDER BY DataApertura DESC";

        return await ExecuteQueryListAsync(query, Array.Empty<SqlParameter>());
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        const string query = @"
            SELECT ID, Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE ID = @ID";

        var parameters = new[] { new SqlParameter("@ID", id) };
        return await ExecuteQuerySingleAsync(query, parameters);
    }

    public async Task<List<Ticket>> GetByCreatoDaIDAsync(int creatoDaID)
    {
        const string query = @"
            SELECT ID, Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE CreatoDaID = @CreatoDaID
            ORDER BY DataApertura DESC";

        var parameters = new[] { new SqlParameter("@CreatoDaID", creatoDaID) };
        return await ExecuteQueryListAsync(query, parameters);
    }

    public async Task<List<Ticket>> GetByAssegnatoAIDAsync(int assegnatoAID)
    {
        const string query = @"
            SELECT ID, Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE AssegnatoAID = @AssegnatoAID
            ORDER BY DataApertura DESC";

        var parameters = new[] { new SqlParameter("@AssegnatoAID", assegnatoAID) };
        return await ExecuteQueryListAsync(query, parameters);
    }

    public async Task<List<Ticket>> GetByStatoAsync(string stato)
    {
        const string query = @"
            SELECT ID, Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE Stato = @Stato
            ORDER BY DataApertura DESC";

        var parameters = new[] { new SqlParameter("@Stato", stato) };
        return await ExecuteQueryListAsync(query, parameters);
    }

    public async Task<List<Ticket>> GetByDateAsync(DateTime data)
    {
        const string query = @"
            SELECT ID, Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE CAST(DataApertura AS DATE) = @DataApertura
            ORDER BY DataApertura DESC";

        var parameters = new[] { new SqlParameter("@DataApertura", data.Date) };
        return await ExecuteQueryListAsync(query, parameters);
    }

    public async Task<List<Ticket>> SearchAsync(string searchTerm)
    {
        const string query = @"
            SELECT ID, Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE Titolo LIKE @SearchTerm 
               OR Descrizione LIKE @SearchTerm 
               OR Stato LIKE @SearchTerm
            ORDER BY DataApertura DESC";

        var parameters = new[] { new SqlParameter("@SearchTerm", $"%{searchTerm}%") };
        return await ExecuteQueryListAsync(query, parameters);
    }

    public async Task<int> InsertAsync(Ticket ticket)
    {
        const string query = @"
            INSERT INTO Ticket (Titolo, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura) 
            VALUES (@Titolo, @Descrizione, @Stato, @CreatoDaID, @AssegnatoAID, @DataApertura);
            SELECT SCOPE_IDENTITY();";

        var parameters = new[]
        {
            new SqlParameter("@Titolo", ticket.Titolo),
            new SqlParameter("@Descrizione", ticket.Descrizione),
            new SqlParameter("@Stato", ticket.Stato),
            new SqlParameter("@CreatoDaID", ticket.CreatoDaID),
            new SqlParameter("@AssegnatoAID", (object?)ticket.AssegnatoAID ?? DBNull.Value),
            new SqlParameter("@DataApertura", DateTime.Now)
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
            throw new DAOException("Errore durante l'inserimento del ticket", ex);
        }
    }
    public async Task UpdateAsync(Ticket ticket)
    {
        const string query = @"
            UPDATE Ticket 
            SET Titolo = @Titolo,
                Descrizione = @Descrizione, 
                Stato = @Stato, 
                CreatoDaID = @CreatoDaID, 
                AssegnatoAID = @AssegnatoAID 
            WHERE ID = @ID";

        var parameters = new[]
        {
            new SqlParameter("@ID", ticket.ID),
            new SqlParameter("@Titolo", ticket.Titolo),
            new SqlParameter("@Descrizione", ticket.Descrizione),
            new SqlParameter("@Stato", ticket.Stato),
            new SqlParameter("@CreatoDaID", ticket.CreatoDaID),
            new SqlParameter("@AssegnatoAID", (object?)ticket.AssegnatoAID ?? DBNull.Value)
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
                throw new DAOException($"Nessun ticket trovato con ID {ticket.ID}");
            }
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante l'aggiornamento del ticket {ticket.ID}", ex);
        }
    }
    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM Ticket WHERE ID = @ID";
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
                throw new DAOException($"Nessun ticket trovato con ID {id}");
            }
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante l'eliminazione del ticket {id}", ex);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        const string query = "SELECT 1 FROM Ticket WHERE ID = @ID";
        var parameters = new[] { new SqlParameter("@ID", id) };
        return await ExistsAsync(query, parameters);
    }

    protected override Ticket MapFromReader(SqlDataReader reader)
    {
        return new Ticket
        {
            ID = reader.GetInt32(reader.GetOrdinal("ID")),
            Titolo = reader.GetString(reader.GetOrdinal("Titolo")),
            Descrizione = reader.GetString(reader.GetOrdinal("Descrizione")),
            Stato = reader.GetString(reader.GetOrdinal("Stato")),
            CreatoDaID = reader.GetInt32(reader.GetOrdinal("CreatoDaID")),
            AssegnatoAID = reader.IsDBNull(reader.GetOrdinal("AssegnatoAID"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("AssegnatoAID")),
            DataApertura = reader.GetDateTime(reader.GetOrdinal("DataApertura"))
        };
    }
}
