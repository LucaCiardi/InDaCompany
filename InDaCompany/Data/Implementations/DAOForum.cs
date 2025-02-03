using InDaCompany.Data.Interfaces;
using Microsoft.Data.SqlClient;
namespace InDaCompany.Data.Implementations;

public class DAOForum : BaseDao<Forum>, IDAOForum
{
    public DAOForum(string connectionString) : base(connectionString) { }
    public List<Forum> GetAll()
    {
        var forums = new List<Forum>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Nome, Descrizione, Team FROM Forum", conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            forums.Add(new Forum
            {
                ID = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descrizione = reader.IsDBNull(2) ? null : reader.GetString(2),
                Team = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }
        return forums;
    }

    public Forum GetById(int id)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Nome, Descrizione, Team FROM Forum WHERE ID = @ID", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Forum
            {
                ID = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descrizione = reader.IsDBNull(2) ? null : reader.GetString(2),
                Team = reader.IsDBNull(3) ? null : reader.GetString(3)
            };
        }
        return null;
    }

    public void Insert(Forum forum)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("INSERT INTO Forum (Nome, Descrizione, Team) VALUES (@Nome, @Descrizione, @Team)", conn);
        cmd.Parameters.AddWithValue("@Nome", forum.Nome);
        cmd.Parameters.AddWithValue("@Descrizione", forum.Descrizione);
        cmd.Parameters.AddWithValue("@Team", forum.Team);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Update(Forum forum)
    {
        using var conn = CreateConnection();
        using var cmd =
            new SqlCommand("UPDATE Forum SET Nome = @Nome, Descrizione = @Descrizione, Team = @Team WHERE ID = @ID",
                conn);
        cmd.Parameters.AddWithValue("@ID", forum.ID);
        cmd.Parameters.AddWithValue("@Nome", forum.Nome);
        cmd.Parameters.AddWithValue("@Descrizione", forum.Descrizione);
        cmd.Parameters.AddWithValue("@Team", forum.Team);
        conn.Open();
        cmd.ExecuteNonQuery();
    }
    public void Delete(int id)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("DELETE FROM Forum WHERE ID = @ID", conn);
        cmd.Parameters.AddWithValue("@ID", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }
    public bool Exists(int id)
    {
        return Exists("SELECT 1 FROM Forum WHERE ID = @ID",
            new[] { new SqlParameter("@ID", id) });
    }

}
