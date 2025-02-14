using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.Data.SqlClient;

public class DAOThreadForum : DAOBase<ThreadForum>, IDAOThreadForum
{
    public DAOThreadForum(string connectionString) : base(connectionString) { }

    public async Task<List<ThreadForum>> GetAllAsync()
    {
        const string query = @"
            SELECT ID, Titolo, Testo, ForumID, AutoreID, DataCreazione 
            FROM ThreadForum
            ORDER BY DataCreazione DESC";

        return await ExecuteQueryListAsync(query, Array.Empty<SqlParameter>());
    }

    public async Task<ThreadForum?> GetByIdAsync(int id)
    {
        const string query = @"
            SELECT ID, Titolo, Testo, ForumID, AutoreID, DataCreazione 
            FROM ThreadForum 
            WHERE ID = @ID";

        var parameters = new[] { new SqlParameter("@ID", id) };
        return await ExecuteQuerySingleAsync(query, parameters);
    }

    public async Task<List<ThreadForum>> GetThreadsByForumAsync(int forumID)
    {
        const string query = @"
            SELECT ID, Titolo, Testo, ForumID, AutoreID, DataCreazione 
            FROM ThreadForum 
            WHERE ForumID = @ForumID
            ORDER BY DataCreazione DESC";

        var parameters = new[] { new SqlParameter("@ForumID", forumID) };
        return await ExecuteQueryListAsync(query, parameters);
    }

    public async Task<List<ThreadForum>> GetThreadsByAuthorAsync(int authorID)
    {
        const string query = @"
            SELECT ID, Titolo, Testo, ForumID, AutoreID, DataCreazione 
            FROM ThreadForum 
            WHERE AutoreID = @AutoreID
            ORDER BY DataCreazione DESC";

        var parameters = new[] { new SqlParameter("@AutoreID", authorID) };
        return await ExecuteQueryListAsync(query, parameters);
    }

    public async Task<int> InsertAsync(ThreadForum entity)
    {
        const string query = @"
            INSERT INTO ThreadForum (Titolo, Testo, ForumID, AutoreID, DataCreazione) 
            VALUES (@Titolo, @Testo, @ForumID, @AutoreID, @DataCreazione);
            SELECT SCOPE_IDENTITY();";

        var parameters = new[]
        {
            new SqlParameter("@Titolo", entity.Titolo),
            new SqlParameter("@Testo", entity.Testo),
            new SqlParameter("@ForumID", entity.ForumID),
            new SqlParameter("@AutoreID", entity.AutoreID),
            new SqlParameter("@DataCreazione", DateTime.Now)
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
            throw new DAOException("Errore durante l'inserimento del thread", ex);
        }
    }

    public async Task UpdateAsync(ThreadForum entity)
    {
        const string query = @"
            UPDATE ThreadForum
            SET Titolo = @Titolo, 
                Testo = @Testo,
                ForumID = @ForumID, 
                AutoreID = @AutoreID
            WHERE ID = @ID";

        var parameters = new[]
        {
            new SqlParameter("@ID", entity.ID),
            new SqlParameter("@Titolo", entity.Titolo),
            new SqlParameter("@Testo", entity.Testo),
            new SqlParameter("@ForumID", entity.ForumID),
            new SqlParameter("@AutoreID", entity.AutoreID)
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
                throw new DAOException($"Nessun thread trovato con ID {entity.ID}");
            }
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante l'aggiornamento del thread {entity.ID}", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM ThreadForum WHERE ID = @ID";
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
                throw new DAOException($"Nessun thread trovato con ID {id}");
            }
        }
        catch (SqlException ex)
        {
            throw new DAOException($"Errore durante l'eliminazione del thread {id}", ex);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        const string query = "SELECT 1 FROM ThreadForum WHERE ID = @ID";
        var parameters = new[] { new SqlParameter("@ID", id) };
        return await ExistsAsync(query, parameters);
    }

    protected override ThreadForum MapFromReader(SqlDataReader reader)
    {
        return new ThreadForum
        {
            ID = reader.GetInt32(reader.GetOrdinal("ID")),
            Titolo = reader.GetString(reader.GetOrdinal("Titolo")),
            Testo = reader.GetString(reader.GetOrdinal("Testo")),
            ForumID = reader.GetInt32(reader.GetOrdinal("ForumID")),
            AutoreID = reader.GetInt32(reader.GetOrdinal("AutoreID")),
            DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione"))
        };
    }
}
