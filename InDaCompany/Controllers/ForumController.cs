using InDaCompany.Data.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InDaCompany.Models;
using InDaCompany.Data.Interfaces;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class ForumController : BaseController
    {
        private readonly IDAOMessaggiThread _daoMessaggiThread;
        private readonly IDAOThreadForum _daoThreadForum;
        private readonly IDAOForum _daoForum;
        private readonly ILogger<HomeController> logger;

        public ForumController(
            IConfiguration configuration,
            ILogger<HomeController> Logger,
            IDAOForum daoForum,
            IDAOThreadForum daoThreadForum,
            IDAOMessaggiThread daoMessaggiThread)
            : base(configuration, Logger)
        {
            _daoForum = daoForum;
            _daoThreadForum = daoThreadForum;
            _daoMessaggiThread = daoMessaggiThread;
            logger = Logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                logger.LogInformation("Accessing home page");
                var forums = await _daoForum.GetAllAsync();
                var threads = await _daoThreadForum.GetAllAsync();
                var messages = await _daoMessaggiThread.GetAllAsync();

                var model = new ForumViewModel
                {
                    Forum = forums ?? new List<Forum>(),
                    Threads = threads ?? new List<ThreadForum>(),
                    Messages = messages ?? new List<MessaggioThread>()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving forums");
                return HandleException(ex);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Forum forum)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _daoForum.InsertAsync(forum);
                    logger.LogInformation("Forum created successfully: {ForumName}", forum.Nome);
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error creating forum");
                    ModelState.AddModelError("", "Unable to save changes. Try again.");
                }
            }
            return View(forum);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var forum = await _daoForum.GetByIdAsync(id);
                if (forum == null)
                {
                    logger.LogWarning("Forum not found: {Id}", id);
                    return NotFound();
                }
                return View(forum);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving forum for edit: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Forum forum)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _daoForum.UpdateAsync(forum);
                    logger.LogInformation("Forum updated successfully: {Id}", forum.ID);
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error updating forum: {Id}", forum.ID);
                    ModelState.AddModelError("", "Unable to save changes. Try again.");
                }
            }
            return View(forum);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var forum = await _daoForum.GetByIdAsync(id);
                if (forum == null)
                {
                    logger.LogWarning("Forum not found for deletion: {Id}", id);
                    return NotFound();
                }
                return View(forum);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving forum for deletion: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _daoForum.DeleteAsync(id);
                logger.LogInformation("Forum deleted successfully: {Id}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error deleting forum: {Id}", id);
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> CreateThread(int forumId)
        {
            try
            {
                var forum = await _daoForum.GetByIdAsync(forumId);
                if (forum == null)
                {
                    return NotFound();
                }

                var threadForum = new ThreadForum { ForumID = forumId };
                return View(threadForum);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error preparing thread creation for forum: {Id}", forumId);
                return HandleException(ex);
            }
        }
    }
}
