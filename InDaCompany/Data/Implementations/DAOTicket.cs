using InDaCompany.Data.Interfaces;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementations;

public class DAOTicket : BaseDao<Ticket>, IDAOTicket
{
    public DAOTicket(string connectionString) : base(connectionString) { }

    public List<Ticket> GetAll()
    {
        var tickets = new List<Ticket>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura FROM Ticket", conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(new Ticket
            {
                ID = reader.GetInt32(0),
                Descrizione = reader.GetString(1),
                Stato = reader.GetString(2),
                CreatoDaID = reader.GetInt32(3),
                AssegnatoAID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                DataApertura = reader.GetDateTime(5)
            });
        }
        return tickets;
    }

    public Ticket GetById(int id)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura FROM Ticket WHERE ID = @ID", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Ticket
            {
                ID = reader.GetInt32(0),
                Descrizione = reader.GetString(1),
                Stato = reader.GetString(2),
                CreatoDaID = reader.GetInt32(3),
                AssegnatoAID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                DataApertura = reader.GetDateTime(5)
            };
        }
        return null;
    }

    public List<Ticket> GetByCreatoDaID(int creatoDaID)
    {
        var tickets = new List<Ticket>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura FROM Ticket WHERE CreatoDaID = @CreatoDaID", conn);
        cmd.Parameters.AddWithValue("@CreatoDaID", creatoDaID);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(new Ticket
            {
                ID = reader.GetInt32(0),
                Descrizione = reader.GetString(1),
                Stato = reader.GetString(2),
                CreatoDaID = reader.GetInt32(3),
                AssegnatoAID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                DataApertura = reader.GetDateTime(5)
            });
        }
        return tickets;
    }

    public List<Ticket> GetByAssegnatoAID(int assegnatoAID)
    {
        var tickets = new List<Ticket>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura FROM Ticket WHERE AssegnatoAID = @AssegnatoAID", conn);
        cmd.Parameters.AddWithValue("@AssegnatoAID", assegnatoAID);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(new Ticket
            {
                ID = reader.GetInt32(0),
                Descrizione = reader.GetString(1),
                Stato = reader.GetString(2),
                CreatoDaID = reader.GetInt32(3),
                AssegnatoAID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                DataApertura = reader.GetDateTime(5)
            });
        }
        return tickets;
    }
    public List<Ticket> GetByStato(string stato)
    {
        var tickets = new List<Ticket>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura FROM Ticket WHERE Stato = @Stato", conn);
        cmd.Parameters.AddWithValue("@Stato", stato);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(new Ticket
            {
                ID = reader.GetInt32(0),
                Descrizione = reader.GetString(1),
                Stato = reader.GetString(2),
                CreatoDaID = reader.GetInt32(3),
                AssegnatoAID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                DataApertura = reader.GetDateTime(5)
            });
        }
        return tickets;
    }

    public List<Ticket> GetByDate(DateTime data)
    {
        var tickets = new List<Ticket>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura FROM Ticket WHERE DataApertura = @DataApertura", conn);
        cmd.Parameters.AddWithValue("@DataApertura", data);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(new Ticket
            {
                ID = reader.GetInt32(0),
                Descrizione = reader.GetString(1),
                Stato = reader.GetString(2),
                CreatoDaID = reader.GetInt32(3),
                AssegnatoAID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                DataApertura = reader.GetDateTime(5)
            });
        }
        return tickets;
    }

    public List<Ticket> Search(string searchTerm)
    {
        var tickets = new List<Ticket>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand(
            "SELECT ID, Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura FROM Ticket " +
            "WHERE Descrizione LIKE @SearchTerm OR Stato LIKE @SearchTerm", conn);
        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tickets.Add(new Ticket
            {
                ID = reader.GetInt32(0),
                Descrizione = reader.GetString(1),
                Stato = reader.GetString(2),
                CreatoDaID = reader.GetInt32(3),
                AssegnatoAID = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                DataApertura = reader.GetDateTime(5)
            });
        }
        return tickets;
    }

    public void Insert(Ticket ticket)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand(
            "INSERT INTO Ticket (Descrizione, Stato, CreatoDaID, AssegnatoAID, DataApertura) VALUES (@Descrizione, @Stato, @CreatoDaID, @AssegnatoAID, @DataApertura)", conn);
        cmd.Parameters.AddWithValue("@Descrizione", ticket.Descrizione);
        cmd.Parameters.AddWithValue("@Stato", ticket.Stato);
        cmd.Parameters.AddWithValue("@CreatoDaID", ticket.CreatoDaID);
        cmd.Parameters.AddWithValue("@AssegnatoAID", ticket.AssegnatoAID);
        cmd.Parameters.AddWithValue("@DataApertura", ticket.DataApertura);
        conn.Open();
        cmd.ExecuteNonQuery();

    }

    public void Update(Ticket ticket)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand(
            "UPDATE Ticket SET Descrizione = @Descrizione, Stato = @Stato, CreatoDaID = @CreatoDaID, AssegnatoAID = @AssegnatoAID, DataApertura = @DataApertura WHERE ID = @ID", conn);
        cmd.Parameters.AddWithValue("@ID", ticket.ID);
        cmd.Parameters.AddWithValue("@Descrizione", ticket.Descrizione);
        cmd.Parameters.AddWithValue("@Stato", ticket.Stato);
        cmd.Parameters.AddWithValue("@CreatoDaID", ticket.CreatoDaID);
        cmd.Parameters.AddWithValue("@AssegnatoAID", ticket.AssegnatoAID);
        cmd.Parameters.AddWithValue("@DataApertura", ticket.DataApertura);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("DELETE FROM Ticket WHERE ID = @ID", conn);
        cmd.Parameters.AddWithValue("@ID", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public bool Exists(int id)
    {
        return Exists("SELECT 1 FROM Ticket WHERE ID = @ID",
            new[] { new SqlParameter("@ID", id) });
    }
}
