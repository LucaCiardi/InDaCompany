using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[Authorize]
public class HomeController : BaseController
{
    private readonly IDAOThreadForum _daoThreadForum;
    private readonly IDAOForum _daoForum;

    public HomeController(
        ILogger<HomeController> logger,
        IDAOThreadForum daoThreadForum,
        IDAOForum daoForum)
        : base(logger)
    {
        _daoThreadForum = daoThreadForum;
        _daoForum = daoForum;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            Logger.LogInformation("Accessing home page");

            var threads = await _daoThreadForum.GetAllAsync();
            var forums = await _daoForum.GetAllAsync();

            var model = new HomeViewModel
            {
                Threads = threads,
                Forums = forums
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
}
