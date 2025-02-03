using InDaCompany.Data.Interfaces;
using Microsoft.Data.SqlClient;
namespace InDaCompany.Data.Implementations;

public class DAOPost : BaseDao<Post>, IDAOPost
{
    public DAOPost(string connectionString) : base(connectionString) { }
    public List<Post> GetAll()
    {
        var posts = new List<Post>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Testo, DataCreazione, AutoreID FROM Post", conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            posts.Add(new Post
            {
                ID = reader.GetInt32(0),
                Testo = reader.GetString(1),
                DataCreazione = reader.GetDateTime(2),
                AutoreID = reader.GetInt32(3)
            });
        }
        return posts;
    }
    public Post GetById(int id)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Testo, DataCreazione, AutoreID FROM Post WHERE ID = @ID", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Post()
            {
                ID = reader.GetInt32(0),
                Testo = reader.GetString(1),
                DataCreazione = reader.GetDateTime(2),
                AutoreID = reader.GetInt32(3)
            };
        }
        return null;
    }
    public List<Post> GetByAutoreID(int autoreID)
    {
        var posts = new List<Post>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Testo, DataCreazione, AutoreID FROM Post WHERE AutoreID = @AutoreID", conn);
        cmd.Parameters.AddWithValue("@AutoreID", autoreID);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            posts.Add(new Post
            {
                ID = reader.GetInt32(0),
                Testo = reader.GetString(1),
                DataCreazione = reader.GetDateTime(2),
                AutoreID = reader.GetInt32(3)
            });
        }
        return posts;
    }

    public List<Post> GetByDataCreazione(DateTime dataCreazione)
    {
        var posts = new List<Post>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Testo, DataCreazione, AutoreID FROM Post WHERE DataCreazione = @DataCreazione", conn);
        cmd.Parameters.AddWithValue("@DataCreazione", dataCreazione);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            posts.Add(new Post
            {
                ID = reader.GetInt32(0),
                Testo = reader.GetString(1),
                DataCreazione = reader.GetDateTime(2),
                AutoreID = reader.GetInt32(3)
            });
        }
        return posts;
    }
    public List<Post> Search(string searchTerm)
    {
        var posts = new List<Post>();
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("SELECT ID, Testo, DataCreazione, AutoreID FROM Post WHERE Testo LIKE @SearchTerm", conn);
        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            posts.Add(new Post
            {
                ID = reader.GetInt32(0),
                Testo = reader.GetString(1),
                DataCreazione = reader.GetDateTime(2),
                AutoreID = reader.GetInt32(3)
            });
        }
        return posts;
    }

    public void Insert(Post post)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("INSERT INTO Post (Testo, AutoreID) VALUES (@Testo, @AutoreID)", conn);
        cmd.Parameters.AddWithValue("@Testo", post.Testo);
        cmd.Parameters.AddWithValue("@AutoreID", post.AutoreID);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Update(Post post)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("UPDATE Post SET Testo = @Testo, DataCreazione = @DataCreazione, AutoreID = @AutoreID WHERE ID = @ID", conn);
        cmd.Parameters.AddWithValue("@ID", post.ID);
        cmd.Parameters.AddWithValue("@Testo", post.Testo);
        cmd.Parameters.AddWithValue("@DataCreazione", post.DataCreazione);
        cmd.Parameters.AddWithValue("@AutoreID", post.AutoreID);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = CreateConnection();
        using var cmd = new SqlCommand("DELETE FROM Post WHERE ID = @ID", conn);
        cmd.Parameters.AddWithValue("@ID", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public bool Exists(int id)
    {
        return Exists("SELECT 1 FROM Post WHERE ID = @ID",
            new[] { new SqlParameter("@ID", id) });
    }
}
