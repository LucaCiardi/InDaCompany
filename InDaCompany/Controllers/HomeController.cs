using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

[Authorize]
public class HomeController : BaseController
{
    private readonly IDAOThreadForum _daoThreadForum;
    private readonly IDAOForum _daoForum;
    private readonly IDAOTicket _daoTicket;
    private readonly IDAOMessaggiThread _daoMessaggiThread;

    public HomeController(
        ILogger<HomeController> logger,
        IDAOThreadForum daoThreadForum,
        IDAOForum daoForum,
        IDAOTicket daoTicket,
        IDAOMessaggiThread daoMessaggiThread)
        : base(logger)
    {
        _daoThreadForum = daoThreadForum;
        _daoForum = daoForum;
        _daoTicket = daoTicket;
        _daoMessaggiThread = daoMessaggiThread;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            int userId = GetCurrentUserId();

            Logger.LogInformation("Accessing home page");

            var threads = await _daoThreadForum.GetAllAsync();
            var forums = await _daoForum.GetAllAsync();
            var tickets = await _daoTicket.GetByAssegnatoAIDAsync(userId);

            foreach ( var thread in threads )
            {
                thread.Messages = await _daoMessaggiThread.GetMessagesByThreadAsync(thread.ID);
            }

            var model = new HomeViewModel
            {
                Threads = threads,
                Forums = forums,
                Tickets = tickets
            };

            return View(model);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error accessing home page");
            return HandleException(ex);
        }
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error([FromQuery] string? message)
    {
        var errorViewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ErrorMessage = message
        };

        Logger.LogError("Error page accessed. RequestId: {RequestId}", errorViewModel.RequestId);
        return View(errorViewModel);
    }
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            Logger.LogWarning("Tentativo di accesso con utente non autenticato correttamente");
            throw new InvalidOperationException("Utente non autenticato correttamente");
        }
        return userId;
    }

}
