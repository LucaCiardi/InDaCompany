using InDaCompany.Data.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers;

public class ThreadsController : Controller
{
    private readonly IDAOThreads _daoThreads;

    public ThreadsController(IConfiguration configuration, IDAOThreads daoThreads)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _daoThreads = daoThreads;
    }

    public IActionResult Index()
    {
        var thread = _daoThreads.GetAll();
        return View(threads);
    }

    public IActionResult Create()
    {
        return View(new Thread());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Thread thread)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _daoThreads.Insert(thread, thread.ForumID, User.GetUserId());
                TempData["Success"] = "Thread creato con successo!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore nella creazione del thread: {ex.Message}";
                return View(thread);
            }
        }
        return View(thread);
    }
    public IActionResult Edit(int id)
    {
        var thread = _daoThreads.GetById(id);
        if (thread == null)
        {
            return NotFound();
        }
        return View(thread);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Thread thread)
    {
        if (id != thread.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _daoThreads.Update(thread);
                TempData["Success"] = "Thread modificato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore durante la modifica del thread: {ex.Message}";
                return View(thread);
            }
        }
        return View(thread);
    }

    public IActionResult Delete(int id)
    {
        var thread = _daoThreads.GetById(id);
        if (thread == null)
        {
            return NotFound();
        }
        return View(thread);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _daoThreads.Delete(id);
            TempData["Success"] = "Thread eliminato con successo";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Errore durante l'eliminazione del thread: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
