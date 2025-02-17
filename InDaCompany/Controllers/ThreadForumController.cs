using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InDaCompany.Models;
using InDaCompany.Data.Interfaces;
using InDaCompany.Data.Implementations;
using System.Diagnostics.Metrics;
using System;

namespace InDaCompany.Controllers
{
    [Authorize]
    public class ThreadForumController : BaseController
    {
        private readonly IDAOThreadForum _daoThread;
        private readonly IDAOForum _daoForum;
        private readonly ILogger<ThreadForumController> _logger;

        public ThreadForumController(
            ILogger<ThreadForumController> logger,
            IDAOThreadForum daoThread,
            IDAOForum daoForum)
            : base(logger)
        {
            _daoThread = daoThread;
            _daoForum = daoForum;
            _logger = logger;
        }
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(int id)
        {
            try
            {
                var thread = await _daoThread.GetByIdAsync(id);
                if (thread?.Immagine != null)
                {
                    return File(thread.Immagine, "image/jpeg"); 
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving thread image: {Id}", id);
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var forumList = await _daoForum.GetAllAsync();
                if (forumList == null)
                {
                    return NotFound();
                }

                ViewBag.Forums = forumList;
                var viewModel = new ThreadCreateViewModel {};
                return View(viewModel);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione del thread");
                return HandleException(ex);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThreadCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var forumList = await _daoForum.GetAllAsync();
                    ViewBag.Forums = forumList;

                    var thread = new ThreadForum
                    {
                        Titolo = model.Titolo,
                        Testo = model.Testo,
                        ForumID = model.ForumID,
                        AutoreID = int.Parse(User.FindFirst("UserId")?.Value ??
                            throw new InvalidOperationException("Utente non autenticato"))
                    };

                    if (model.Immagine != null)
                    {
                        using var memoryStream = new MemoryStream();
                        await model.Immagine.CopyToAsync(memoryStream);
                        thread.Immagine = memoryStream.ToArray();
                    }
                    else
                    {
                        // Load default image
                        var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot", "images", "nopicture.png");
                        thread.Immagine = await System.IO.File.ReadAllBytesAsync(defaultImagePath);
                    }

                    await _daoThread.InsertAsync(thread);
                    return RedirectToAction("Index", "Forum", new { id = thread.ForumID });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione del thread");
                    ModelState.AddModelError("", "Impossibile creare il thread. Riprova.");
                }
            }

            try
            {
                var forum = await _daoForum.GetByIdAsync(model.ForumID);
                ViewBag.ForumName = forum?.Nome ?? "Forum sconosciuto";
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del nome del forum");
            }

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThreadForum thread, IFormFile? NewImage, bool RemoveImage = false, string? returnUrl = null)
        {
            _logger.LogInformation("Edit started - RemoveImage flag: {RemoveImage}", RemoveImage);

            if (id != thread.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var originalThread = await _daoThread.GetByIdAsync(id);
                    if (originalThread == null) return NotFound();

                    // Set other properties
                    thread.Titolo = thread.Titolo;
                    thread.Testo = thread.Testo;
                    thread.ForumID = originalThread.ForumID;
                    thread.AutoreID = originalThread.AutoreID;
                    thread.DataCreazione = originalThread.DataCreazione;

                    if (NewImage != null)
                    {
                        _logger.LogInformation("Processing new image upload");
                        using var memoryStream = new MemoryStream();
                        await NewImage.CopyToAsync(memoryStream);
                        thread.Immagine = memoryStream.ToArray();
                    }
                    else if (RemoveImage)
                    {
                        _logger.LogInformation("Setting default image");
                        var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot", "images", "nopicture.png");

                        if (!System.IO.File.Exists(defaultImagePath))
                        {
                            _logger.LogError("Default image not found at: {Path}", defaultImagePath);
                            throw new FileNotFoundException("Default image not found", defaultImagePath);
                        }

                        thread.Immagine = await System.IO.File.ReadAllBytesAsync(defaultImagePath);
                        _logger.LogInformation("Default image set successfully");
                    }
                    else
                    {
                        _logger.LogInformation("Keeping original image");
                        thread.Immagine = originalThread.Immagine;
                    }

                    await _daoThread.UpdateAsync(thread);
                    _logger.LogInformation("Thread updated successfully");

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Forum");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating thread: {Id}", id);
                    ModelState.AddModelError("", "Errore durante l'aggiornamento del thread");
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid");
            }

            return View(thread);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var thread = await _daoThread.GetByIdAsync(id);
                if (thread == null) return NotFound();

                if (thread.AutoreID != int.Parse(User.FindFirst("UserId")?.Value ?? "0"))
                {
                    return Forbid();
                }

                return View(thread);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving thread: {Id}", id);
                return HandleException(ex);
            }
        }

    }
}
