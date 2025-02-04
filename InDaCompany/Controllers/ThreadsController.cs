using InDaCompany.Data.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers;

public class ThreadsController : Controller
{
    private readonly DAOThreads _daoThreads;

    public ThreadsController(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _daoThreads = new DAOThreads(connectionString);
    }

    public IActionResult Index()
    {
        var thread = _daoThreads.GetAll();
        return View(thread);
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
                _daoThreads.Insert(thread);
                TempData["Success"] = "Thread created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating thread: {ex.Message}";
            }
        }

        ViewBag.Negozi = _daoThreads.GetAll();
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
        if (id != thread.ManagedThreadId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _daoThreads.Update(thread);
                TempData["Success"] = "Thread updated successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating thread: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
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
            TempData["Success"] = "Thread deleted successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting thread: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
