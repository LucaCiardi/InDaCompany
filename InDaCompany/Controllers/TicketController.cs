using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using InDaCompany.Data.Implementations;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        private readonly IDAOTicket _daoTicket;
        private readonly ILogger<TicketController> _logger;
        private readonly IDAOUtenti _daoUtenti;

        public TicketController(
            ILogger<TicketController> logger,
            IDAOTicket daoTicket,
            IDAOUtenti daoUtenti)
            : base(logger)
        {
            _daoTicket = daoTicket;
            _logger = logger;
            _daoUtenti = daoUtenti;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                int userId = GetCurrentUserId();
                var user = await _daoUtenti.GetByIdAsync(userId);
                if (user.Ruolo == "Admin" || user.Ruolo == "Manager")
                {
                    var tickets = await _daoTicket.GetAllAsync();
                    return View(tickets);
                }
                else
                {
                    var tickets = await _daoTicket.GetByAssegnatoAIDAsync(userId);
                    tickets.Concat(await _daoTicket.GetByCreatoDaIDAsync(userId));
                    return View(tickets);
                }
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei ticket");
                return HandleException(ex);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Ticket { Stato = "Aperto" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titolo,Descrizione,AssegnatoAID")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Stato = "Aperto";
                    ticket.CreatoDaID = GetCurrentUserId();
                    await _daoTicket.InsertAsync(ticket);

                    _logger.LogInformation("Ticket creato con successo dall'utente {UserId}", ticket.CreatoDaID);
                    TempData["Success"] = "Ticket creato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione del ticket");
                    ModelState.AddModelError("", "Impossibile salvare le modifiche. Riprovare.");
                }
            }
            return View(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                int userId = GetCurrentUserId();
                var user = await _daoUtenti.GetByIdAsync(userId);

                var ticket = await _daoTicket.GetByIdAsync(id);
                if (ticket == null)
                {
                    _logger.LogWarning("Ticket non trovato: {Id}", id);
                    return NotFound();
                }
                ViewBag.InfoUtente = user;
                return View(ticket);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del ticket: {Id}", id);
                return HandleException(ex);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Titolo,Descrizione,Stato,AssegnatoAID,Soluzione")] Ticket ticket)
        {
            if (id != ticket.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var originalTicket = await _daoTicket.GetByIdAsync(ticket.ID);
                    if (originalTicket == null) return NotFound();

                    ticket.DataApertura = originalTicket.DataApertura;
                    ticket.CreatoDaID = originalTicket.CreatoDaID;

                    if (ticket.Stato == "Chiuso")
                    {
                        if (string.IsNullOrEmpty(ticket.Soluzione))
                        {
                            ModelState.AddModelError("Soluzione", "La soluzione è obbligatoria per i ticket chiusi");
                            return View(ticket);
                        }
                        ticket.DataChiusura = DateTime.Now;
                    }

                    await _daoTicket.UpdateAsync(ticket);
                    TempData["Success"] = "Ticket modificato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento del ticket: {Id}", ticket.ID);
                    ModelState.AddModelError("", "Impossibile salvare le modifiche. Riprovare.");
                }
            }
            return View(ticket);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ticket = await _daoTicket.GetByIdAsync(id);
                if (ticket == null)
                {
                    _logger.LogWarning("Ticket non trovato per l'eliminazione: {Id}", id);
                    return NotFound();
                }
                return View(ticket);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del ticket per l'eliminazione: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _daoTicket.DeleteAsync(id);
                TempData["Success"] = "Ticket eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ticket: {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione del ticket";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ByCreator(int creatorId)
        {
            try
            {
                var tickets = await _daoTicket.GetByCreatoDaIDAsync(creatorId);
                return View("Index", tickets);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei ticket per creatore: {CreatorId}", creatorId);
                return HandleException(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ByAssignee(int assigneeId)
        {
            try
            {
                var tickets = await _daoTicket.GetByAssegnatoAIDAsync(assigneeId);
                return View("Index", tickets);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei ticket assegnati: {AssigneeId}", assigneeId);
                return HandleException(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ByStatus(string status)
        {
            try
            {
                var tickets = await _daoTicket.GetByStatoAsync(status);
                return View("Index", tickets);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei ticket per stato: {Status}", status);
                return HandleException(ex);
            }
        }

        [HttpGet]
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
                _logger.LogError(ex, "Errore durante la ricerca dei ticket con termine: {Term}", term);
                return HandleException(ex);
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
