using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOLikes : IDAOBase<Like>
    {
        Task<int> GetLikeCountAsync(int threadID);
        Task<bool> HasUserLikedPostAsync(int utenteID, int threadID);
        Task<int> ToggleLikeAsync(int utenteID, int threadID);
        Task DeleteByPostAndUserAsync(int utenteID, int threadID);
    }
}