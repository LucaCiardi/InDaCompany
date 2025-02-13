using System.Security.Claims;
using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class ProfileController(
        IConfiguration configuration,
        IDAOUtenti DAOUtenti,
        IDAOTicket DAOTicket,
        IDAOPost DAOPost,
        IDAOForum DAOForum,
        ILogger<ProfileController> logger) : BaseController(configuration, logger)
    {
        private readonly IDAOUtenti _daoUtenti = DAOUtenti;
        private readonly IDAOTicket _daoTicket = DAOTicket;
        private readonly IDAOPost _daoPost = DAOPost;
        private readonly IDAOForum _daoForum = DAOForum;

        public async Task<IActionResult> Index()
        {
            try
            {
                int userId = GetCurrentUserId();
                var user = await _daoUtenti.GetByIdAsync(userId);

                if (user == null)
                    return NotFound();

                var ticketsByAuthor = await _daoTicket.GetByCreatoDaIDAsync(userId);
                //var postsByAuthor = await _daoPost.GetByAutoreIDAsync(userId);
                var forumsOfAuthor = await _daoForum.GetForumByUser(user.Team == null ? "Generale" : user.Team);

                ProfileViewModel profileModel = new ProfileViewModel {
                    Utente = user,
                    Tickets = ticketsByAuthor,
                    Forums = forumsOfAuthor
                };

                return View(profileModel);
            }
            catch (DAOException ex)
            {
                Logger.LogError(ex, "Error retrieving profile data");
                return HandleException(ex);
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User not properly authenticated");
            }
            return userId;
        }

        public IActionResult CreateTicket()
        {
            return RedirectToAction("Create", "Ticket");
        }

        public IActionResult CreatePost()
        {
            return RedirectToAction("Create", "Post");
        }

        private async Task<Dictionary<string, int>> GetUserStats(int userId)
        {
            return new Dictionary<string, int>
        {
            { "TotalTickets", (await _daoTicket.GetByCreatoDaIDAsync(userId)).Count },
            { "TotalPosts", (await _daoPost.GetByAutoreIDAsync(userId)).Count }
        };
        }
    }

}
