using InDaCompany.Data.Implementation;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;
using Thread = System.Threading.Thread;

namespace InDaCompany.Controllers;

public class MessaggiThreadController : Controller
{
    private readonly DAOMessaggiThread _daoMessaggiThread;

    public MessaggiThreadController(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _daoMessaggiThread = new DAOMessaggiThread(connectionString);
    }

    public IActionResult Index()
    {
        var threadMes = _daoMessaggiThread.GetAll();
        return View(threadMes);
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
                _daoMessaggiThread.Insert(threadMes);
                TempData["Success"] = "Messagge created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating messagge: {ex.Message}";
            }
        }

        ViewBag.Negozi = _daoMessaggiThread.GetAll();
        return View(threadMes);
    }
    public IActionResult Edit(int id)
    {
        var threadMes = _daoMessaggiThread.GetById(id);
        if (threadMes == null)
        {
            return NotFound();
        }
        return View(threadMes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, MessaggioThread threadMes)
    {
        if (id != threadMes.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _daoMessaggiThread.Update(threadMes);
                TempData["Success"] = "Message updated successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating message: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
        return View(threadMes);
    }

    public IActionResult Delete(int id)
    {
        var threadMes = _daoMessaggiThread.GetById(id);
        if (threadMes == null)
        {
            return NotFound();
        }
        return View(threadMes);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _daoMessaggiThread.Delete(id);
            TempData["Success"] = "Message deleted successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting message: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
