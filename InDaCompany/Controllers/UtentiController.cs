using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UtentiController(
        IConfiguration configuration,
        IDAOUtenti DAOUtenti,
        ILogger<UtentiController> logger) : BaseController(configuration, logger)
    {
        private readonly IDAOUtenti _daoUtenti = DAOUtenti;

        public async Task<IActionResult> Index()
        {
            try
            {
                var utenti = await _daoUtenti.GetAllAsync();
                return View(utenti);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving users");
                return HandleException(ex);
            }
        }

        public IActionResult Create()
        {
            return View(new Utente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Utente utente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _daoUtenti.InsertAsync(utente);
                    logger.LogInformation("User created successfully: {Email}", utente.Email);
                    TempData["Success"] = "Utente creato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error creating user: {Email}", utente.Email);
                    TempData["Error"] = "Errore nella creazione dell'utente";
                    return View(utente);
                }
            }
            return View(utente);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var utente = await _daoUtenti.GetByIdAsync(id);
                if (utente == null)
                {
                    logger.LogWarning("User not found: {Id}", id);
                    return NotFound();
                }
                return View(utente);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving user for edit: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Utente utente)
        {
            if (id != utente.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _daoUtenti.GetByIdAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    if (string.IsNullOrEmpty(utente.PasswordHash))
                    {
                        utente.PasswordHash = existingUser.PasswordHash;
                    }

                    await _daoUtenti.UpdateAsync(utente);
                    logger.LogInformation("User updated successfully: {Id}", id);
                    TempData["Success"] = "Utente modificato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error updating user: {Id}", id);
                    TempData["Error"] = "Errore durante la modifica dell'utente";
                    return View(utente);
                }
            }
            return View(utente);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var utente = await _daoUtenti.GetByIdAsync(id);
                if (utente == null)
                {
                    logger.LogWarning("User not found for deletion: {Id}", id);
                    return NotFound();
                }

                if (utente.Ruolo == "Admin" && !await HasOtherAdminsAsync(id))
                {
                    TempData["Error"] = "Impossibile eliminare l'ultimo amministratore";
                    return RedirectToAction(nameof(Index));
                }

                return View(utente);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving user for deletion: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var utente = await _daoUtenti.GetByIdAsync(id);
                if (utente == null)
                {
                    return NotFound();
                }

                if (utente.Ruolo == "Admin" && !await HasOtherAdminsAsync(id))
                {
                    TempData["Error"] = "Impossibile eliminare l'ultimo amministratore";
                    return RedirectToAction(nameof(Index));
                }

                await _daoUtenti.DeleteAsync(id);
                logger.LogInformation("User deleted successfully: {Id}", id);
                TempData["Success"] = "Utente eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error deleting user: {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione dell'utente";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<bool> HasOtherAdminsAsync(int currentUserId)
        {
            try
            {
                var allUsers = await _daoUtenti.GetAllAsync();
                return allUsers.Any(u => u.Ruolo == "Admin" && u.ID != currentUserId);
            }
            catch (DAOException)
            {
                return true;
            }
        }
    }
}
