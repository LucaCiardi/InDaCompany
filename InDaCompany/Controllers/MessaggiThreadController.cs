using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class MessaggiThreadController : BaseController
{
    private readonly IDAOMessaggiThread _daoMessaggiThread;
    private readonly ILogger<MessaggiThreadController> _logger;

    public MessaggiThreadController(
        ILogger<MessaggiThreadController> logger,
        IDAOMessaggiThread daoMessaggiThread)
        : base(logger)
    {
        _daoMessaggiThread = daoMessaggiThread;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MessaggioThread message, string? returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            try
            {
                message.AutoreID = int.Parse(User.FindFirst("UserId")?.Value ??
                    throw new InvalidOperationException("Utente non autenticato"));

                await _daoMessaggiThread.InsertAsync(message);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Forum");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del messaggio");
                ModelState.AddModelError("", "Errore durante la creazione del messaggio");
            }
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Forum");
    }
}
