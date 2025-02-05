using System.Diagnostics;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    public class HomeController(
        IConfiguration configuration,
        ILogger<HomeController> logger) : BaseController(configuration, logger)
    {
        public IActionResult Index()
        {
            try
            {
                logger.LogInformation("Accessing home page");
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error accessing home page");
                return HandleException(ex);
            }
        }

        public IActionResult Privacy()
        {
            try
            {
                logger.LogInformation("Accessing privacy page");
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error accessing privacy page");
                return HandleException(ex);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            logger.LogError("Error page accessed. RequestId: {RequestId}", errorViewModel.RequestId);
            return View(errorViewModel);
        }
    }
}
