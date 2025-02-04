using InDaCompany.Data.Implementation;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;
using Thread = System.Threading.Thread;

namespace InDaCompany.Controllers;

public class MessaggiThreadController : Controller
{
    private readonly IDAOMessaggiThread _daoMessaggiThread;

    public MessaggiThreadController(IConfiguration configuration, IDAOMessaggiThread daoMessaggiThread)
    {
        _daoMessaggiThread = daoMessaggiThread;
    }

    public IActionResult Index()
    {
        var threadMes = _daoMessaggiThread.GetAll();
        return View(messaggi);
    }

    public IActionResult Create()
    {
        return View(new MessaggioThread());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(MessaggioThread threadMes)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _daoMessaggiThread.Insert(messaggio, messaggio.ThreadID, User.GetUserId());
                TempData["Success"] = "Messaggio creato con successo!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore durante la creazioen del messaggio: {ex.Message}";
                return View(messaggio);
            }
        }
        return View(messaggio);
    }
    public IActionResult Edit(int id)
    {
        var threadMes = _daoMessaggiThread.GetById(id);
        if (threadMes == null)
        {
            return NotFound();
        }
        return View(messaggio);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, MessaggioThread messaggio)
    {
        if (id != threadMes.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var original = _daoMessaggiThread.GetById(id);
                if (original == null) return NotFound();
                messaggio.DataCreazione = original.DataCreazione;
                messaggio.AutoreID = original.AutoreID;
                _daoMessaggiThread.Update(messaggio);
                TempData["Success"] = "Messaggio modificato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore durante la modifica del messaggio: {ex.Message}";
                return View(messaggio);
            }
        }
        return View(messaggio);
    }

    public IActionResult Delete(int id)
    {
        var messaggio = _daoMessaggiThread.GetById(id);
        if (messaggio == null)
        {
            return NotFound();
        }
        return View(messaggio);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            var messaggio = _daoMessaggiThread.GetById(id);
            if (messaggio == null) return NotFound();
            _daoMessaggiThread.Delete(id);
            TempData["Success"] = "Messaggio eliminato con successo";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Errore durante l'eliminazione del messaggio: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
