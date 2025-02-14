﻿using Microsoft.AspNetCore.Authorization;
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

        public TicketController(
            ILogger<TicketController> logger,
            IDAOTicket daoTicket)
            : base(logger)
        {
            _daoTicket = daoTicket;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var tickets = await _daoTicket.GetAllAsync();
                return View(tickets);
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
                var ticket = await _daoTicket.GetByIdAsync(id);
                if (ticket == null)
                {
                    _logger.LogWarning("Ticket non trovato: {Id}", id);
                    return NotFound();
                }
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Titolo,Descrizione,Stato,AssegnatoAID")] Ticket ticket)
        {
            if (id != ticket.ID)
            {
                return NotFound();
            }

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
                    _logger.LogInformation("Ticket aggiornato con successo: {Id}", ticket.ID);
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

        [HttpPost, ActionName("Delete")]
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
                _logger.LogInformation("Ticket eliminato con successo: {Id}", id);
                TempData["Success"] = "Ticket eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del ticket: {Id}", id);
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
