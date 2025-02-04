using InDaCompany.Data.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InDaCompany.Models;

namespace InDaCompany.Controllers;

[Authorize]
public class TicketController : BaseController
{
   private readonly IDAOTicket _daoTicket;

   public TicketController(IConfiguration configuration, IDAOTicket daoTicket) : base(configuration)
   {
       _daoTicket = daoTicket;
   }
    public ActionResult Index()
    {
         var tickets = _daoTicket.GetAll();
         return View(tickets);
    }
    public ActionResult Create()
    {
        return View(new Ticket { Stato = "Aperto" });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Ticket ticket)
    {
        if (ModelState.IsValid)
        {
            try
            {
                ticket.Stato = "Aperto";
                _daoTicket.Insert(ticket);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Impossibile salvare le modifiche. Riprovare.");
            }
        }
        return View(ticket);
    }

    public ActionResult Edit(int id)
    {
        var ticket = _daoTicket.GetById(id);
        if (ticket == null) return NotFound();
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Ticket ticket)
    {
        var originalTicket = _daoTicket.GetById(ticket.ID);
        if (originalTicket == null) return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                ticket.DataApertura = originalTicket.DataApertura;
                ticket.CreatoDaID = originalTicket.CreatoDaID;
                _daoTicket.Update(ticket);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Impossibile salvare le modifiche. Riprovare.");
            }
        }
        return View(ticket);
    }

    public ActionResult Delete(int id)
    {
        var ticket = _daoTicket.GetById(id);
        if (ticket == null) return NotFound();
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        try
        {
            var ticket = _daoTicket.GetById(id);
            if (ticket == null) return NotFound();
            _daoTicket.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    public ActionResult ByCreator(int creatorId)
    {
        var tickets = _daoTicket.GetByCreatoDaID(creatorId);
        return View("Index", tickets);
    }

    public ActionResult ByAssignee(int assigneeId)
    {
        var tickets = _daoTicket.GetByAssegnatoAID(assigneeId);
        return View("Index", tickets);
    }

    public ActionResult ByStatus(string status)
    {
        var tickets = _daoTicket.GetByStato(status);
        return View("Index", tickets);
    }

    public ActionResult Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return RedirectToAction(nameof(Index));
            
        var tickets = _daoTicket.Search(term);
        return View("Index", tickets);
    }

}
