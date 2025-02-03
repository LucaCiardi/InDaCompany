using InDaCompany.Data.Interfaces;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementation
{
    public class DAOThreads : DAOBase<Thread>, IDAOThreads
    {
        public DAOThreads(string connectionString) : base(connectionString)
        {
        }

        public void Delete(int id)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("DELETE FROM Thread WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool Exists(int id)
        {
            return Exists("SELECT 1 FROM Thread WHERE ID = @ID",
                new[] { new SqlParameter("@ID", id) });
        }

        public List<Thread> GetAll()
        {
            var threads = new List<Thread>();
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("SELECT ID, Titolo, ForumID, AutoreID, DataCreazione FROM Thread", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                threads.Add(new Thread
                {
                    ID = reader.GetInt32(0),
                    Titolo = reader.GetString(1),
                    ForumID = reader.GetInt32(2),
                    AutoreID = reader.GetInt32(3),
                    DataCreazione = reader.GetDateTime(4)
                });
            }
            return threads;
        }

        public Thread GetById(int id)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("SELECT ID, Titolo, ForumID, AutoreID, DataCreazione FROM Thread WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Thread
                {
                    ID = reader.GetInt32(0),
                    Titolo = reader.GetString(1),
                    ForumID = reader.GetInt32(2),
                    AutoreID = reader.GetInt32(3),
                    DataCreazione = reader.GetDateTime(4)
                };
            }
            return null;
        }

        public void Insert(Thread entity, int forumID, int autoreID)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("INSERT INTO Thread (Titolo, ForumID, AutoreID) VALUES (@Titolo, @ForumID, @AutoreID)",
        conn);

            cmd.Parameters.AddWithValue("@Titolo", entity.Titolo);
            cmd.Parameters.AddWithValue("@ForumID", ForumID);
            cmd.Parameters.AddWithValue("@AutoreID", AutoreID);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(Thread entity)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(@"
                UPDATE Thread
                SET 
                    Titolo = @Titolo, 
                    ForumID = @ForumID, 
                    AutoreID = @AutoreID, 
                    DataCreazione = @DataCreazione 
                WHERE ID = @ID", conn);

            cmd.Parameters.AddWithValue("@ID", entity.ID);
            cmd.Parameters.AddWithValue("@Titolo", entity.Titolo);
            cmd.Parameters.AddWithValue("@ForumID", entity.ForumID);
            cmd.Parameters.AddWithValue("@AutoreID", entity.AutoreID);
            
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
