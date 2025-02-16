using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

public class DAOLikes : DAOBase<Like>, IDAOLikes
{
    public DAOLikes(string connectionString) : base(connectionString) { }

    public async Task<List<Like>> GetAllAsync()
    {
        const string query = "SELECT ID, UtenteID, ThreadID, MiPiace, DataLike FROM Likes ORDER BY DataLike DESC";
        return await ExecuteQueryListAsync(query, Array.Empty<SqlParameter>());
    }

    public async Task<Like?> GetByIdAsync(int id)
    {
        const string query = "SELECT ID, UtenteID, ThreadID, MiPiace, DataLike FROM Likes WHERE ID = @ID";
        var parameters = new[] { new SqlParameter("@ID", id) };
        return await ExecuteQuerySingleAsync(query, parameters);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        const string query = "SELECT 1 FROM Likes WHERE ID = @ID";
        var parameters = new[] { new SqlParameter("@ID", id) };
        return await ExistsAsync(query, parameters);
    }

    public async Task<int> InsertAsync(Like like)
    {
        const string query = @"
            INSERT INTO Likes (UtenteID, ThreadID, MiPiace, DataLike) 
            VALUES (@UtenteID, @ThreadID, @MiPiace, @DataLike);
            SELECT SCOPE_IDENTITY();";

        var parameters = new[]
        {
            new SqlParameter("@UtenteID", like.UtenteID),
            new SqlParameter("@ThreadID", like.ThreadID),
            new SqlParameter("@MiPiace", like.MiPiace),
            new SqlParameter("@DataLike", DateTime.Now)
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
            throw new DAOException("Errore durante l'inserimento del like", ex);
        }
    }
    public async Task UpdateAsync(Like like)
    {
        const string query = @"
        UPDATE Likes 
        SET UtenteID = @UtenteID,
            ThreadID = @ThreadID,
            MiPiace = @MiPiace,
            DataLike = @DataLike
        WHERE ID = @ID";

        var parameters = new[]
        {
            new SqlParameter("@ID", like.ID),
            new SqlParameter("@UtenteID", like.UtenteID),
            new SqlParameter("@ThreadID", like.ThreadID),
            new SqlParameter("@MiPiace", like.MiPiace),
            new SqlParameter("@DataLike", like.DataLike)
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
                throw new DAOException($"Nessun like trovato con ID {like.ID}");
            }
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante l'aggiornamento del like {like.ID}", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM Likes WHERE ID = @ID";
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
                throw new DAOException($"Nessun like trovato con ID {id}");
            }
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante l'eliminazione del like {id}", ex);
        }
    }
    public async Task<int> GetLikeCountAsync(int threadID)
    {
        const string query = "SELECT COUNT(*) FROM Likes WHERE ThreadID = @ThreadID AND MiPiace = 1";

        var parameters = new[] { new SqlParameter("@ThreadID", threadID) };

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddRange(parameters);

        try
        {
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante il conteggio dei like per il thread {threadID}", ex);
        }
    }

    public async Task<bool> HasUserLikedPostAsync(int utenteID, int threadID)
    {
        const string query = "SELECT 1 FROM Likes WHERE UtenteID = @UtenteID AND ThreadID = @ThreadID AND MiPiace = 1";

        var parameters = new[]
        {
            new SqlParameter("@UtenteID", utenteID),
            new SqlParameter("@ThreadID", threadID)
        };

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddRange(parameters);

        try
        {
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante la verifica del like per l'utente {utenteID} nel thread {threadID}", ex);
        }
    }
    public async Task<int> ToggleLikeAsync(int utenteID, int threadID)
    {
        const string query = @"
        IF EXISTS (SELECT 1 FROM Likes WHERE UtenteID = @UtenteID AND ThreadID = @ThreadID)
        BEGIN
            UPDATE Likes 
            SET MiPiace = CASE WHEN MiPiace = 1 THEN 0 ELSE 1 END,
                DataLike = @DataLike
            WHERE UtenteID = @UtenteID AND ThreadID = @ThreadID;
            SELECT MiPiace FROM Likes WHERE UtenteID = @UtenteID AND ThreadID = @ThreadID;
        END
        ELSE
        BEGIN
            INSERT INTO Likes (UtenteID, ThreadID, MiPiace, DataLike) 
            VALUES (@UtenteID, @ThreadID, 1, @DataLike);
            SELECT 1;
        END";

        var parameters = new[]
        {
            new SqlParameter("@UtenteID", utenteID),
            new SqlParameter("@ThreadID", threadID),
            new SqlParameter("@DataLike", DateTime.Now)
        };

        using var conn = CreateConnection();
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddRange(parameters);

        try
        {
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante il toggle del like per l'utente {utenteID} nel thread {threadID}", ex);
        }
    }

    public async Task DeleteByPostAndUserAsync(int utenteID, int threadID)
    {
        const string query = "DELETE FROM Likes WHERE UtenteID = @UtenteID AND ThreadID = @ThreadID";

        var parameters = new[]
        {
            new SqlParameter("@UtenteID", utenteID),
            new SqlParameter("@ThreadID", threadID)
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
            throw new DAOException($"Errore durante l'eliminazione del like per l'utente {utenteID} nel thread {threadID}", ex);
        }
    }

    protected override Like MapFromReader(SqlDataReader reader)
    {
        return new Like
        {
            ID = reader.GetInt32(reader.GetOrdinal("ID")),
            UtenteID = reader.GetInt32(reader.GetOrdinal("UtenteID")),
            ThreadID = reader.GetInt32(reader.GetOrdinal("ThreadID")),
            MiPiace = reader.GetBoolean(reader.GetOrdinal("MiPiace")),
            DataLike = reader.GetDateTime(reader.GetOrdinal("DataLike"))
        };
    }
}
