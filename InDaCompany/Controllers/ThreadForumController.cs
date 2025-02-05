using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InDaCompany.Models;
using InDaCompany.Data.Interfaces;
using InDaCompany.Data.Implementations;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class ThreadForumController(
        IConfiguration configuration,
        IDAOThreadForum DAOThread,
        IDAOForum DAOForum,
        ILogger<ThreadForumController> logger) : BaseController(configuration, logger)
    {
        private readonly IDAOForum _daoForum = DAOForum;

        public async Task<IActionResult> Create(int forumId)
        {
            try
            {
                var forum = await _daoForum.GetByIdAsync(forumId);
                if (forum == null)
                {
                    return NotFound();
                }

                ViewBag.ForumName = forum.Nome;
                return View(new ThreadForum { ForumID = forumId });
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error preparing thread creation for forum: {Id}", forumId);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThreadForum thread)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    thread.AutoreID = int.Parse(User.FindFirst("UserId")?.Value ??
                        throw new InvalidOperationException("User not authenticated"));

                    await DAOThread.InsertAsync(thread);
                    logger.LogInformation("Thread created successfully in forum: {ForumId}", thread.ForumID);
                    return RedirectToAction("Details", "Forum", new { id = thread.ForumID });
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error creating thread");
                    ModelState.AddModelError("", "Unable to create thread. Please try again.");
                }
            }

            try
            {
                var forum = await _daoForum.GetByIdAsync(thread.ForumID);
                ViewBag.ForumName = forum?.Nome ?? "Unknown Forum";
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving forum name");
            }

            return View(thread);
        }
    }
}
