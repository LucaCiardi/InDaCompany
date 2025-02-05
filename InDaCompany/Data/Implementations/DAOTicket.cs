using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations;

public class DAOTicket(string connectionString) : DAOBase<Ticket>(connectionString), IDAOTicket
{
    public async Task<List<Ticket>> GetAllAsync()
    {
        const string query = @"
            SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket";
        var tickets = new List<Ticket>();

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);

        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(MapFromReader(reader));
            }
            return tickets;
        }
        catch (SqlException ex)
        {
            throw new DAOException("Error retrieving tickets", ex);
        }
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        const string query = @"
            SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE ID = @ID";
        var parameters = new[] { new SqlParameter("@ID", id) };

        return await ExecuteQuerySingleAsync(query, parameters);
    }

    public async Task<List<Ticket>> GetByCreatoDaIDAsync(int creatoDaID)
    {
        const string query = @"
            SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE CreatoDaID = @CreatoDaID";
        var tickets = new List<Ticket>();

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@CreatoDaID", creatoDaID);

        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(MapFromReader(reader));
            }
            return tickets;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Error retrieving tickets for creator {creatoDaID}", ex);
        }
    }

    public async Task<List<Ticket>> GetByAssegnatoAIDAsync(int assegnatoAID)
    {
        const string query = @"
            SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE AssegnatoAID = @AssegnatoAID";
        var tickets = new List<Ticket>();

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@AssegnatoAID", assegnatoAID);

        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(MapFromReader(reader));
            }
            return tickets;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Error retrieving tickets assigned to {assegnatoAID}", ex);
        }
    }

    public async Task<List<Ticket>> GetByStatoAsync(string stato)
    {
        const string query = @"
            SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE Stato = @Stato";
        var tickets = new List<Ticket>();

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Stato", stato);

        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(MapFromReader(reader));
            }
            return tickets;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Error retrieving tickets with state {stato}", ex);
        }
    }

    public async Task<List<Ticket>> GetByDateAsync(DateTime data)
    {
        const string query = @"
            SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE CAST(DataApertura AS DATE) = @DataApertura";
        var tickets = new List<Ticket>();

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@DataApertura", data.Date);

        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(MapFromReader(reader));
            }
            return tickets;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Error retrieving tickets for date {data:d}", ex);
        }
    }

    public async Task<List<Ticket>> SearchAsync(string searchTerm)
    {
        const string query = @"
            SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura 
            FROM Ticket 
            WHERE Descrizione LIKE @SearchTerm 
               OR Stato LIKE @SearchTerm";
        var tickets = new List<Ticket>();

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(MapFromReader(reader));
            }
            return tickets;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Error searching tickets with term '{searchTerm}'", ex);
        }
    }

    public async Task<int> InsertAsync(Ticket ticket)
    {
        const string query = @"
            INSERT INTO Ticket (Descrizione, Stato, CreatoDaID, AssegnatoAID) 
            VALUES (@Descrizione, @Stato, @CreatoDaID, @AssegnatoAID);
            SELECT SCOPE_IDENTITY();";

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@Descrizione", ticket.Descrizione);
        cmd.Parameters.AddWithValue("@Stato", ticket.Stato);
        cmd.Parameters.AddWithValue("@CreatoDaID", ticket.CreatoDaID);
        cmd.Parameters.AddWithValue("@AssegnatoAID", (object?)ticket.AssegnatoAID ?? DBNull.Value);

        try
        {
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        catch (SqlException ex)
        {
            throw new DAOException("Error inserting ticket", ex);
        }
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        const string query = @"
            UPDATE Ticket 
            SET Descrizione = @Descrizione, 
                Stato = @Stato, 
                CreatoDaID = @CreatoDaID, 
                AssegnatoAID = @AssegnatoAID 
            WHERE ID = @ID";

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@ID", ticket.ID);
        cmd.Parameters.AddWithValue("@Descrizione", ticket.Descrizione);
        cmd.Parameters.AddWithValue("@Stato", ticket.Stato);
        cmd.Parameters.AddWithValue("@CreatoDaID", ticket.CreatoDaID);
        cmd.Parameters.AddWithValue("@AssegnatoAID", (object?)ticket.AssegnatoAID ?? DBNull.Value);

        try
        {
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Error updating ticket {ticket.ID}", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM Ticket WHERE ID = @ID";

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
            throw new DAOException($"Error deleting ticket {id}", ex);
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
