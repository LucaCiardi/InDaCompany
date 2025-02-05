using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    public abstract class BaseController(IConfiguration configuration, ILogger<BaseController> logger) : Controller
    {
        protected readonly string ConnectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        protected readonly ILogger<BaseController> Logger = logger;

        protected IActionResult HandleException(Exception ex)
        {
            Logger.LogError(ex, "An error occurred in the controller");
            return StatusCode(500, "An unexpected error occurred");
        }
    }
}
