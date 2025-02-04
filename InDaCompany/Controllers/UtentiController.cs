using InDaCompany.Data.Implementation;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;


namespace InDaCompany.Controllers;
[Authorize]
public class UtentiController : Controller
{
    private readonly IDAOUtenti _daoUtenti;

    public UtentiController(IConfiguration configuration, IDAOUtenti daoUtenti)
    {
        _daoUtenti = daoUtenti;
    }

    public IActionResult Index()
    {
        var utente = _daoUtenti.GetAll();
        return View(utenti);
    }

    public IActionResult Create()
    {
        return View(new Utente());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Utente utente)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _daoUtenti.Insert(utente);
                TempData["Success"] = "Utente creato con successo!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore nella creazione del thread: {ex.Message}";
                return View(utente);
            }
        }

        return View(utente);
    }
    public IActionResult Edit(int id)
    {
        var utente = _daoUtenti.GetById(id);
        if (utente == null)
        {
            return NotFound();
        }
        return View(utente); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Utente utente)
    {
        if (id != utente.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _daoUtenti.Update(utente);
                TempData["Success"] = "Utente modificato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore durante la modifica dell'utente: {ex.Message}";
                return View(utente);
            }
        }
        return View(utente);
    }

    public IActionResult Delete(int id)
    {
        var utente = _daoUtenti.GetById(id);
        if (utente == null)
        {
            return NotFound();
        }
        return View(utente);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _daoUtenti.Delete(id);
            TempData["Success"] = "Utente cancellato con successo";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Errore durante la cancellazione dell'utente: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
