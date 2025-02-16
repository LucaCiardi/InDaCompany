using System.Security.Claims;
using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly IDAOUtenti _daoUtenti;
        private readonly IDAOTicket _daoTicket;
        private readonly IDAOForum _daoForum;
        private readonly IDAOThreadForum _daoThreadForum;
        private readonly ILogger<ProfileController> _logger;
        private readonly IDAOMessaggiThread _daoMessaggiThread;

        public ProfileController(
            ILogger<ProfileController> logger,
            IDAOUtenti daoUtenti,
            IDAOTicket daoTicket,
            IDAOForum daoForum,
            IDAOThreadForum daoThreadForum,
        IDAOMessaggiThread daoMessaggiThread)
            : base(logger)
        {
            _daoUtenti = daoUtenti;
            _daoTicket = daoTicket;
            _daoForum = daoForum;
            _daoThreadForum = daoThreadForum;
            _logger = logger;
            _daoMessaggiThread = daoMessaggiThread;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                int userId = GetCurrentUserId();
                var user = await _daoUtenti.GetByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("Utente non trovato: {UserId}", userId);
                    return NotFound();
                }

                // Get all user's data
                var ticketsByAuthor = await _daoTicket.GetByCreatoDaIDAsync(userId);
                var threadsByAuthor = await _daoThreadForum.GetThreadsByAuthorAsync(userId);
                var forumsOfAuthor = await _daoForum.GetForumByUser(user.Email);
                var messages = await _daoMessaggiThread.GetMessagesByAuthorAsync(userId);

                // Load messages for each thread
                foreach (var thread in threadsByAuthor)
                {
                    thread.Messages = await _daoMessaggiThread.GetMessagesByThreadAsync(thread.ID);
                }

                // Load messages for threads where the user commented
                var messageThreadIds = messages.Select(m => m.ThreadID).Distinct();
                var messageThreads = await Task.WhenAll(
                    messageThreadIds.Select(async id => {
                        var thread = await _daoThreadForum.GetByIdAsync(id);
                        if (thread != null)
                        {
                            thread.Messages = await _daoMessaggiThread.GetMessagesByThreadAsync(thread.ID);
                        }
                        return thread;
                    })
                );

                var profileModel = new ProfileViewModel
                {
                    Utente = user,
                    Tickets = ticketsByAuthor,
                    Forums = forumsOfAuthor,
                    ThreadForums = threadsByAuthor,
                    Messages = messages,
                    MessageThreads = messageThreads
                        .Where(t => t != null)
                        .ToDictionary(t => t.ID)
                };

                // Add users for displaying names in comments
                ViewBag.Utenti = await _daoUtenti.GetAllAsync();

                _logger.LogInformation("Profilo recuperato con successo per l'utente: {UserId}", userId);
                return View(profileModel);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei dati del profilo");
                return HandleException(ex);
            }
        }
        [HttpGet]
        public IActionResult CreateTicket()
        {
            return RedirectToAction("Create", "Ticket");
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogWarning("Tentativo di accesso con utente non autenticato correttamente");
                throw new InvalidOperationException("Utente non autenticato correttamente");
            }
            return userId;
        }

        private async Task<Dictionary<string, int>> GetUserStats(int userId)
        {
            try
            {
                return new Dictionary<string, int>
                {
                    { "TotalTickets", (await _daoTicket.GetByCreatoDaIDAsync(userId)).Count },
                    { "TotalThreads", (await _daoThreadForum.GetThreadsByAuthorAsync(userId)).Count },
                    { "TotalForums", (await _daoForum.GetForumByUser(User.FindFirst(ClaimTypes.Email)?.Value ?? "")).Count }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche utente: {UserId}", userId);
                throw;
            }
        }
    }
}
