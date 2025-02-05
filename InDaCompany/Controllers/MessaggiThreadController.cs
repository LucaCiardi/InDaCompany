using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    public class MessaggiThreadController(
        IConfiguration configuration,
        IDAOMessaggiThread DAOMessaggiThread,
        ILogger<MessaggiThreadController> logger) : BaseController(configuration, logger)
    {
        private readonly IDAOMessaggiThread _daoMessaggiThread = DAOMessaggiThread;

        public async Task<IActionResult> Index()
        {
            try
            {
                var messaggi = await _daoMessaggiThread.GetAllAsync();
                return View(messaggi);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving messages");
                return HandleException(ex);
            }
        }

        public IActionResult Create()
        {
            return View(new MessaggioThread());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessaggioThread messaggio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Assuming you have a method to get the current user's ID
                    int userId = GetCurrentUserId();
                    await _daoMessaggiThread.InsertAsync(messaggio);

                    logger.LogInformation("Message created successfully for thread {ThreadId}", messaggio.ThreadID);
                    TempData["Success"] = "Messaggio creato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error creating message");
                    TempData["Error"] = "Errore durante la creazione del messaggio";
                    return View(messaggio);
                }
            }
            return View(messaggio);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var messaggio = await _daoMessaggiThread.GetByIdAsync(id);
                if (messaggio == null)
                {
                    logger.LogWarning("Message not found: {Id}", id);
                    return NotFound();
                }
                return View(messaggio);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving message for edit: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MessaggioThread messaggio)
        {
            if (id != messaggio.ID)
            {
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

                    // Preserve original values
                    messaggio.DataCreazione = original.DataCreazione;
                    messaggio.AutoreID = original.AutoreID;

                    await _daoMessaggiThread.UpdateAsync(messaggio);
                    logger.LogInformation("Message updated successfully: {Id}", id);
                    TempData["Success"] = "Messaggio modificato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error updating message: {Id}", id);
                    TempData["Error"] = "Errore durante la modifica del messaggio";
                    return View(messaggio);
                }
            }
            return View(messaggio);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var messaggio = await _daoMessaggiThread.GetByIdAsync(id);
                if (messaggio == null)
                {
                    logger.LogWarning("Message not found for deletion: {Id}", id);
                    return NotFound();
                }
                return View(messaggio);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving message for deletion: {Id}", id);
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

                await _daoMessaggiThread.DeleteAsync(id);
                logger.LogInformation("Message deleted successfully: {Id}", id);
                TempData["Success"] = "Messaggio eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error deleting message: {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione del messaggio";
                return RedirectToAction(nameof(Index));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User not properly authenticated");
            }
            return userId;
        }
    }
}
