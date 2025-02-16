using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InDaCompany.Data.Interfaces;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class LikeController : BaseController
    {
        private readonly IDAOLikes _daoLikes;
        private readonly ILogger<LikeController> _logger;

        public LikeController(
            ILogger<LikeController> logger,
            IDAOLikes daoLikes)
            : base(logger)
        {
            _daoLikes = daoLikes;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int threadId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ??
                    throw new InvalidOperationException("Utente non autenticato"));

                var result = await _daoLikes.ToggleLikeAsync(userId, threadId);
                var likeCount = await _daoLikes.GetLikeCountAsync(threadId);

                return Json(new { success = true, liked = result == 1, likeCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il toggle del like per il thread {ThreadId}", threadId);
                return Json(new { success = false, message = "Si è verificato un errore" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLikeStatus(int threadId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ??
                    throw new InvalidOperationException("Utente non autenticato"));

                var isLiked = await _daoLikes.HasUserLikedPostAsync(userId, threadId);
                var likeCount = await _daoLikes.GetLikeCountAsync(threadId);

                return Json(new { success = true, liked = isLiked, likeCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dello stato del like per il thread {ThreadId}", threadId);
                return Json(new { success = false, message = "Si è verificato un errore" });
            }
        }
    }
}
