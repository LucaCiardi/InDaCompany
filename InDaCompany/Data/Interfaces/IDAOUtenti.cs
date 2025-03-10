﻿using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOUtenti : IDAOBase<Utente>
    {
        Task<Utente?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<Utente?> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string newPasswordHash);
        Task UpdateProfilePictureAsync(int userId, byte[] imageData);
        Task SetDefaultProfilePictureAsync(int userId);
        Task<List<Utente>> GetByTeamAsync(string team);

    }
}
