using System.Diagnostics;
using InDaCompany.Models;
using InDaCompany.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IDAOForum _daoForum;
        private readonly IDAOThreadForum _daoThreadForum;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IConfiguration configuration,
            ILogger<HomeController> logger,
            IDAOForum daoForum,
            IDAOThreadForum daoThreadForum)
            : base(configuration, logger)
        {
            _daoForum = daoForum;
            _daoThreadForum = daoThreadForum;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {   
            var mailUser = ViewBag.User == null ? "laura.bianchi@azienda.com" : "laura.bianchi@azienda.com";
            try
            {
                _logger.LogInformation("Accessing home page");

                //var posts = await _daoPost.GetAllAsync();
                var threads = await _daoThreadForum.GetHomeThreadsAsync();
                var forums = await _daoForum.GetForumByUser(mailUser);

                var model = new HomeViewModel
                {
                    Forums = forums ?? new List<Forum>(),
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
