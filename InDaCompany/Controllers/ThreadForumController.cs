using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InDaCompany.Models;
using InDaCompany.Data.Interfaces;
using InDaCompany.Data.Implementations;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class ThreadForumController : BaseController
    {
        private readonly IDAOThreadForum _daoThread;
        private readonly IDAOForum _daoForum;
        private readonly ILogger<ThreadForumController> _logger;

        public ThreadForumController(
            ILogger<ThreadForumController> logger,
            IDAOThreadForum daoThread,
            IDAOForum daoForum)
            : base(logger)
        {
            _daoThread = daoThread;
            _daoForum = daoForum;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int forumId)
        {
            try
            {
                var forum = await _daoForum.GetByIdAsync(forumId);
                if (forum == null)
                {
                    _logger.LogWarning("Forum non trovato: {Id}", forumId);
                    return NotFound();
                }

                ViewBag.ForumName = forum.Nome;
                return View(new ThreadForum { ForumID = forumId });
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione del thread per il forum: {Id}", forumId);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ForumID,Titolo,Testo")] ThreadForum thread)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    thread.AutoreID = int.Parse(User.FindFirst("UserId")?.Value ??
                        throw new InvalidOperationException("Utente non autenticato"));

                    await _daoThread.InsertAsync(thread);
                    _logger.LogInformation("Thread creato con successo nel forum: {ForumId}", thread.ForumID);
                    return RedirectToAction("Details", "Forum", new { id = thread.ForumID });
                }
                catch (DAOException ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione del thread");
                    ModelState.AddModelError("", "Impossibile creare il thread. Riprova.");
                }
            }

            try
            {
                var forum = await _daoForum.GetByIdAsync(thread.ForumID);
                ViewBag.ForumName = forum?.Nome ?? "Forum sconosciuto";
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del nome del forum");
            }

            return View(thread);
        }
    }
}
