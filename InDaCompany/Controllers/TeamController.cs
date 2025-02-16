using System.Security.Claims;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class TeamController : BaseController
    {
        private readonly IDAOUtenti _daoUtenti;
        private readonly IDAOTicket _daoTicket;
        private readonly IDAOForum _daoForum;
        private readonly IDAOThreadForum _daoThreadForum;
        private readonly ILogger<TeamController> _logger;

        public TeamController(
            ILogger<TeamController> logger,
            IDAOUtenti daoUtenti,
            IDAOTicket daoTicket,
            IDAOForum daoForum,
            IDAOThreadForum daoThreadForum)
            : base(logger)
        {
            _daoUtenti = daoUtenti;
            _daoTicket = daoTicket;
            _daoForum = daoForum;
            _daoThreadForum = daoThreadForum;
            _logger = logger;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("Utente non autenticato correttamente");
            }
            return userId;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = await _daoUtenti.GetByIdAsync(GetCurrentUserId());
                if (currentUser?.Team == null)
                {
                    return View(new TeamViewModel());
                }

                var teamMembers = await _daoUtenti.GetByTeamAsync(currentUser.Team);
                var teamForums = await _daoForum.GetForumByTeam(currentUser.Team);
                var teamTickets = await _daoTicket.GetByTeamAsync(currentUser.Team);

                var viewModel = new TeamViewModel
                {
                    TeamName = currentUser.Team,
                    Members = teamMembers,
                    Forums = teamForums,
                    Tickets = teamTickets
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei dati del team");
                return HandleException(ex);
            }
        }
    }

}
