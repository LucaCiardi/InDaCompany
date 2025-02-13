using InDaCompany.Data.Implementations;
using Microsoft.AspNetCore.Mvc;

public abstract class BaseController : Controller
{
    protected readonly string ConnectionString;
    protected readonly ILogger<BaseController> Logger;

    protected BaseController(IConfiguration configuration, ILogger<BaseController> logger)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        Logger = logger;
    }

    protected IActionResult HandleException(Exception ex)
    {
        Logger.LogError(ex, "An error occurred in the controller");

        if (ex is DAOException)
        {
            return StatusCode(500, "A database error occurred");
        }

        return StatusCode(500, "An unexpected error occurred");
    }

    protected async Task<IActionResult> HandleExceptionAsync(Exception ex)
    {
        await Task.CompletedTask; 
        return HandleException(ex);
    }
}
