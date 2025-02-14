using Microsoft.AspNetCore.Mvc;

public abstract class BaseController : Controller
{
    protected readonly ILogger Logger;

    protected BaseController(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected IActionResult HandleException(Exception ex)
    {
        Logger.LogError(ex, "Si è verificato un errore nel controller");
        return RedirectToAction("Error", "Home", new { message = "Si è verificato un errore inaspettato" });
    }
}
