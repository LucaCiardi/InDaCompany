using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOLikes : IDAOBase<Like>
    {
        Task<int> GetLikeCountAsync(int postID);
        Task<bool> HasUserLikedPostAsync(int utenteID, int postID);
        Task<int> ToggleLikeAsync(int utenteID, int postID);
        Task DeleteByPostAndUserAsync(int utenteID, int postID);
    }
}

