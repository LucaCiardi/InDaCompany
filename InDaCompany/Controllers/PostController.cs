using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class PostController(
        IConfiguration configuration,
        IDAOPost DAOPost,
        ILogger<PostController> logger) : BaseController(configuration, logger)
    {
        private readonly IDAOPost _daoPost = DAOPost;

        public async Task<IActionResult> Index()
        {
            try
            {
                var posts = await _daoPost.GetAllAsync();
                return View(posts);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving posts");
                return HandleException(ex);
            }
        }

        public IActionResult Create()
        {
            return View(new Post());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    post.AutoreID = GetCurrentUserId();
                    await _daoPost.InsertAsync(post);

                    logger.LogInformation("Post created successfully by user {UserId}", post.AutoreID);
                    TempData["Success"] = "Post creato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error creating post");
                    TempData["Error"] = "Errore durante la creazione del post";
                    return View(post);
                }
            }
            return View(post);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var post = await _daoPost.GetByIdAsync(id);
                if (post == null)
                {
                    logger.LogWarning("Post not found: {Id}", id);
                    return NotFound();
                }

                // Optional: Check if the current user is the author
                if (post.AutoreID != GetCurrentUserId())
                {
                    logger.LogWarning("Unauthorized edit attempt for post {Id} by user {UserId}",
                        id, GetCurrentUserId());
                    return Unauthorized();
                }

                return View(post);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving post for edit: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalPost = await _daoPost.GetByIdAsync(id);
                    if (originalPost == null)
                    {
                        return NotFound();
                    }

                    // Check if the current user is the author
                    if (originalPost.AutoreID != GetCurrentUserId())
                    {
                        return Unauthorized();
                    }

                    // Preserve original values
                    post.DataCreazione = originalPost.DataCreazione;
                    post.AutoreID = originalPost.AutoreID;

                    await _daoPost.UpdateAsync(post);
                    logger.LogInformation("Post updated successfully: {Id}", id);
                    TempData["Success"] = "Post modificato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    logger.LogError(ex, "Error updating post: {Id}", id);
                    TempData["Error"] = "Errore durante la modifica del post";
                    return View(post);
                }
            }
            return View(post);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var post = await _daoPost.GetByIdAsync(id);
                if (post == null)
                {
                    logger.LogWarning("Post not found for deletion: {Id}", id);
                    return NotFound();
                }

                if (post.AutoreID != GetCurrentUserId())
                {
                    return Unauthorized();
                }

                return View(post);
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error retrieving post for deletion: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var post = await _daoPost.GetByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }

                if (post.AutoreID != GetCurrentUserId())
                {
                    return Unauthorized();
                }

                await _daoPost.DeleteAsync(id);
                logger.LogInformation("Post deleted successfully: {Id}", id);
                TempData["Success"] = "Post eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                logger.LogError(ex, "Error deleting post: {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione del post";
                return RedirectToAction(nameof(Index));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User not properly authenticated");
            }
            return userId;
        }
    }
}
