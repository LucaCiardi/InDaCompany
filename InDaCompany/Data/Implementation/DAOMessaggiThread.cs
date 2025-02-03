using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

namespace InDaCompany.Data.Implementation
{
    public class DAOMessaggiThread : DAOBase<MessaggioThread>, IDAOBase<MessaggioThread>
    {
        public DAOMessaggiThread(string connectionString) : base(connectionString)
        {
        }

        public void Delete(int id)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("DELETE FROM MessaggiThread WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool Exists(int id)
        {
            return Exists("SELECT 1 FROM MessaggiThread WHERE ID = @ID",
                new[] { new SqlParameter("@ID", id) });
        }

        public List<MessaggioThread> GetAll()
        {
            var threads = new List<MessaggioThread>();
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("SELECT ID, ThreadID, AutoreID, Testo, DataCreazione FROM MessaggiThread", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                threads.Add(new MessaggioThread
                {
                    ID = reader.GetInt32(0),
                    ThreadID = reader.GetInt32(1),
                    AutoreID = reader.GetInt32(2),
                    Testo = reader.GetString(3),
                    DataCreazione = reader.GetDateTime(4)
                });
            }
            return threads;
        }

        public MessaggioThread GetById(int id)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand("SELECT ID, ThreadID, AutoreID, Testo, DataCreazione FROM MessaggiThread WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new MessaggioThread
                {
                    ID = reader.GetInt32(0),
                    ThreadID = reader.GetInt32(1),
                    AutoreID = reader.GetInt32(2),
                    Testo = reader.GetString(3),
                    DataCreazione = reader.GetDateTime(4)
                };
            }
            return null;
        }

        public void Insert(MessaggioThread entity, int threadID, int autoreID)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(
                "UPDATE MessaggiThread SET " +
                $"ThreadID = {threadID}, " +
                $"AutoreID = {autoreID}, " +
                "Testo = @Testo, " +
                "DataCreazione = @DataCreazione " +
                "WHERE ID = @ID",
                conn);

            cmd.Parameters.AddWithValue("@ID", entity.ID);
            cmd.Parameters.AddWithValue("@ForumID", entity.ThreadID);
            cmd.Parameters.AddWithValue("@AutoreID", entity.AutoreID);
            cmd.Parameters.AddWithValue("@Testo", entity.Testo);
            cmd.Parameters.AddWithValue("@DataCreazione", entity.DataCreazione);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(MessaggioThread entity)
        {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(@"
                UPDATE MessaggiThread
                SET 
                    ThreadID = @ThreadID, 
                    AutoreID = @AutoreID, 
                    Testo = @Testo, 
                    DataCreazione = @DataCreazione 
                WHERE ID = @ID", conn);

            cmd.Parameters.AddWithValue("@ID", entity.ID);
            cmd.Parameters.AddWithValue("@ForumID", entity.ThreadID);
            cmd.Parameters.AddWithValue("@AutoreID", entity.AutoreID);
            cmd.Parameters.AddWithValue("@Testo", entity.Testo);
            cmd.Parameters.AddWithValue("@DataCreazione", entity.DataCreazione);
            cmd.ExecuteNonQuery();

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
