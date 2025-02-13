using System.Diagnostics;
using InDaCompany.Models;
using InDaCompany.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IDAOPost _daoPost;
        private readonly IDAOThreadForum _daoThreadForum;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IConfiguration configuration,
            ILogger<HomeController> logger,
            IDAOPost daoPost,
            IDAOThreadForum daoThreadForum)
            : base(configuration, logger)
        {
            _daoPost = daoPost;
            _daoThreadForum = daoThreadForum;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Accessing home page");

                //var posts = await _daoPost.GetAllAsync();
                var threads = await _daoThreadForum.GetAllAsync();

                var model = new HomeViewModel
                {
                    
                    Threads = threads ?? new List<ThreadForum>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing home page");
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
            try
            {
                _logger.LogInformation("Accessing privacy page");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing privacy page");
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

            _logger.LogError("Error page accessed. RequestId: {RequestId}", errorViewModel.RequestId);
            return View(errorViewModel);
        }
    }
}
