using System.Security.Claims;
using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
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
        ILogger<ProfileController> logger) : BaseController(configuration, logger)
    {
        private readonly IDAOUtenti _daoUtenti = DAOUtenti;
        private readonly IDAOTicket _daoTicket = DAOTicket;
        private readonly IDAOPost _daoPost = DAOPost;

        public async Task<IActionResult> Index()
        {
            try
            {
                int userId = GetCurrentUserId();
                var user = await _daoUtenti.GetByIdAsync(userId);

                if (user == null)
                    return NotFound();

                ViewBag.RecentTickets = await _daoTicket.GetByCreatoDaIDAsync(userId);
                ViewBag.RecentPosts = await _daoPost.GetByAutoreIDAsync(userId);

                return View(user);
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
