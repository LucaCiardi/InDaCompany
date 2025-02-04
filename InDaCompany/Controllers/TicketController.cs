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
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Ticket ticket)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _daoTicket.Insert(ticket);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again.");
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
        if (ModelState.IsValid)
        {
            try
            {
                _daoTicket.Update(ticket);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again.");
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
            _daoTicket.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    // non so come impostare il fatto che si veda lo stato del ticket

}
