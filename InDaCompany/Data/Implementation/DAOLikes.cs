namespace InDaCompany.Data.Implementation {

    public DAOLikes : DAOBase<Like>, IDAOLikes {

        public DAOLikes(string connectionString) : base(connectionString) {

        }

        public void Create(int utenteID, int postID, bool isLiked) {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(
                "INSERT INTO Likes (UtenteID, PostID, MiPiace) VALUES (@UtenteId, @PostId, @IsLiked)",
                conn);

                cmd.Parameters.AddWithValue("@UtenteId", utenteID);
                cmd.Parameters.AddWithValue("@PostId", postID);
                cmd.Parameters.AddWithValue("@IsLiked", isLiked);
                conn.Open();
                cmd.ExecuteNonQuery();
        }

        public void Delete(int utenteID, int postID) {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(
                "DELETE FROM Likes WHERE UtenteID = @UtenteId AND PostID = @PostId",
                conn);
            
            cmd.Parameters.AddWithValue("@UtenteId", utenteID);
            cmd.Parameters.AddWithValue("@PostId", postID);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public int Count(int postID) {
            using var conn = CreateConnection();
            using var cmd = new SqlCommand(
                "SELECT COUNT(*) AS num_likes FROM Likes WHERE PostID = @PostId",
                conn);

            cmd.Parameters.AddWithValue("@PostId", postID);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }
        protected override Like MapFromReader(SqlDataReader reader)
        {
            return new Like
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                UtenteID = reader.GetInt32(reader.GetOrdinal("UtenteID")),
                PostID = reader.GetInt32(reader.GetOrdinal("PostID")),
                MiPiace = reader.GetBoolean(reader.GetOrdinal("MiPiace"))
            };
        }
    }
}
