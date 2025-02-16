using System.Threading;
using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class MessaggiThreadController : BaseController
    {
        private readonly IDAOMessaggiThread _daoMessaggiThread;
        private readonly IDAOThreadForum _daoThreadForum;
        private readonly ILogger<MessaggiThreadController> _logger;

        public MessaggiThreadController(
     ILogger<MessaggiThreadController> logger,
     IDAOMessaggiThread daoMessaggiThread,
     IDAOThreadForum daoThreadForum)
     : base(logger)
        {
            _daoMessaggiThread = daoMessaggiThread;
            _daoThreadForum = daoThreadForum;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var messaggi = await _daoMessaggiThread.GetAllAsync();
                return View(messaggi);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei messaggi");
                return HandleException(ex);
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new MessaggioThread());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ThreadID,Testo")] MessaggioThread messaggio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    messaggio.AutoreID = GetCurrentUserId();
                    await _daoMessaggiThread.InsertAsync(messaggio);

                    // Get the thread to find its forum
                    var thread = await _daoThreadForum.GetByIdAsync(messaggio.ThreadID);
                    if (thread == null)
                    {
                        return NotFound();
                    }

                    _logger.LogInformation("Messaggio creato con successo nel thread {ThreadId}", messaggio.ThreadID);
                    TempData["Success"] = "Commento aggiunto con successo";

                    // Redirect back to the forum where the comment was made
                    return RedirectToAction("Index", "Forum");  // Or if you have a Details action: return RedirectToAction("Details", "Forum", new { id = thread.ForumID });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione del messaggio");
                    TempData["Error"] = "Errore durante l'invio del commento";
                    return RedirectToAction("Index", "Forum");
                }
            }

            TempData["Error"] = "Dati non validi";
            return RedirectToAction("Index", "Forum");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var messaggio = await _daoMessaggiThread.GetByIdAsync(id);
                if (messaggio == null)
                {
                    _logger.LogWarning("Messaggio non trovato: {Id}", id);
                    return NotFound();
                }

                if (messaggio.AutoreID != GetCurrentUserId())
                {
                    _logger.LogWarning("Tentativo non autorizzato di modificare il messaggio: {Id}", id);
                    return Forbid();
                }

                return View(messaggio);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del messaggio: {Id}", id);
                return HandleException(ex);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ThreadID,Testo")] MessaggioThread messaggio)
        {
            if (id != messaggio.ID)
            {
                _logger.LogWarning("ID messaggio non corrispondente");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var original = await _daoMessaggiThread.GetByIdAsync(id);
                    if (original == null)
                    {
                        return NotFound();
                    }

                    if (original.AutoreID != GetCurrentUserId())
                    {
                        _logger.LogWarning("Tentativo non autorizzato di modificare il messaggio: {Id}", id);
                        return Forbid();
                    }

                    messaggio.DataCreazione = original.DataCreazione;
                    messaggio.AutoreID = original.AutoreID;

                    await _daoMessaggiThread.UpdateAsync(messaggio);
                    _logger.LogInformation("Messaggio aggiornato con successo: {Id}", id);
                    TempData["Success"] = "Messaggio modificato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento del messaggio: {Id}", id);
                    TempData["Error"] = "Errore durante la modifica del messaggio";
                    return View(messaggio);
                }
            }
            return View(messaggio);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var messaggio = await _daoMessaggiThread.GetByIdAsync(id);
                if (messaggio == null)
                {
                    _logger.LogWarning("Messaggio non trovato per l'eliminazione: {Id}", id);
                    return NotFound();
                }

                if (messaggio.AutoreID != GetCurrentUserId())
                {
                    _logger.LogWarning("Tentativo non autorizzato di eliminare il messaggio: {Id}", id);
                    return Forbid();
                }

                return View(messaggio);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del messaggio per l'eliminazione: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var messaggio = await _daoMessaggiThread.GetByIdAsync(id);
                if (messaggio == null)
                {
                    return NotFound();
                }

                if (messaggio.AutoreID != GetCurrentUserId())
                {
                    _logger.LogWarning("Tentativo non autorizzato di eliminare il messaggio: {Id}", id);
                    return Forbid();
                }

                await _daoMessaggiThread.DeleteAsync(id);
                _logger.LogInformation("Messaggio eliminato con successo: {Id}", id);
                TempData["Success"] = "Messaggio eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del messaggio: {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione del messaggio";
                return RedirectToAction(nameof(Index));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("Utente non autenticato correttamente");
            }
            return userId;
        }
    }
}
