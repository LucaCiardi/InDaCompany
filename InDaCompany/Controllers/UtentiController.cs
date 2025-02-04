using InDaCompany.Data.Implementation;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;


namespace InDaCompany.Controllers;

public class UtentiController : Controller
{
    private readonly DAOUtenti _daoUtenti;

    public UtentiController(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _daoUtenti = new DAOUtenti(connectionString);
    }

    public IActionResult Index()
    {
        var utente = _daoUtenti.GetAll();
        return View(utente);
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
                TempData["Success"] = "Thread created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating thread: {ex.Message}";
            }
        }

        ViewBag.Negozi = _daoUtenti.GetAll();
        return View(utente);
    }
    public IActionResult Edit(int id)
    {
        var utente = _daoUtenti.GetById(id);
        if (utente == null)
        {
            return NotFound();
        }
        return View(""); // vuole una stringa, se scrivo View(utente) mi da errore
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
                TempData["Success"] = "User updated successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating user: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
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
        return View("");
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _daoUtenti.Delete(id);
            TempData["Success"] = "Thread deleted successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting thread: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
