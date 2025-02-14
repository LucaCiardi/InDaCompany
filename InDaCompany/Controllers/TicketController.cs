using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class TicketController(
        IConfiguration configuration,
        IDAOTicket DAOTicket,
        ILogger<TicketController> logger) : BaseController(configuration, logger)
    {
        private readonly IDAOTicket _daoTicket = DAOTicket;

        public async Task<IActionResult> Index()
        {
            try
            {
                var tickets = await _daoTicket.GetAllAsync();
                return View(tickets);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving tickets");
                return HandleException(ex);
            }
        }

        public IActionResult Create()
        {
            return View(new Ticket { Stato = "Aperto" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Stato = "Aperto";
                    ticket.CreatoDaID = GetCurrentUserId();
                    await _daoTicket.InsertAsync(ticket);

                    logger.LogInformation("Ticket created successfully by user {UserId}", ticket.CreatoDaID);
                    TempData["Success"] = "Ticket creato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error creating ticket");
                    ModelState.AddModelError("", "Impossibile salvare le modifiche. Riprovare.");
                }
            }
            return View(ticket);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var ticket = await _daoTicket.GetByIdAsync(id);
                if (ticket == null)
                {
                    logger.LogWarning("Ticket not found: {Id}", id);
                    return NotFound();
                }
                return View(ticket);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving ticket for edit: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var originalTicket = await _daoTicket.GetByIdAsync(ticket.ID);
                    if (originalTicket == null)
                    {
                        return NotFound();
                    }

                    ticket.DataApertura = originalTicket.DataApertura;
                    ticket.CreatoDaID = originalTicket.CreatoDaID;

                    await _daoTicket.UpdateAsync(ticket);
                    logger.LogInformation("Ticket updated successfully: {Id}", ticket.ID);
                    TempData["Success"] = "Ticket modificato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error updating ticket: {Id}", ticket.ID);
                    ModelState.AddModelError("", "Impossibile salvare le modifiche. Riprovare.");
                }
            }
            return View(ticket);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ticket = await _daoTicket.GetByIdAsync(id);
                if (ticket == null)
                {
                    logger.LogWarning("Ticket not found for deletion: {Id}", id);
                    return NotFound();
                }
                return View(ticket);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving ticket for deletion: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ticket = await _daoTicket.GetByIdAsync(id);
                if (ticket == null)
                {
                    return NotFound();
                }

                await _daoTicket.DeleteAsync(id);
                logger.LogInformation("Ticket deleted successfully: {Id}", id);
                TempData["Success"] = "Ticket eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error deleting ticket: {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione del ticket";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> ByCreator(int creatorId)
        {
            try
            {
                var tickets = await _daoTicket.GetByCreatoDaIDAsync(creatorId);
                return View("Index", tickets);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving tickets by creator: {CreatorId}", creatorId);
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> ByAssignee(int assigneeId)
        {
            try
            {
                var tickets = await _daoTicket.GetByAssegnatoAIDAsync(assigneeId);
                return View("Index", tickets);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving tickets by assignee: {AssigneeId}", assigneeId);
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> ByStatus(string status)
        {
            try
            {
                var tickets = await _daoTicket.GetByStatoAsync(status);
                return View("Index", tickets);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving tickets by status: {Status}", status);
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var tickets = await _daoTicket.SearchAsync(term);
                return View("Index", tickets);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error searching tickets with term: {Term}", term);
                return HandleException(ex);
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
