using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using InDaCompany.Data.Implementations;

namespace InDaCompany.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UtentiController : BaseController
    {
        private readonly IDAOUtenti _daoUtenti;
        private readonly ILogger<UtentiController> _logger;

        public UtentiController(
            ILogger<UtentiController> logger,
            IDAOUtenti daoUtenti)
            : base(logger)
        {
            _daoUtenti = daoUtenti;
            _logger = logger;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Cognome,Email,PasswordHash,Ruolo,Team")] Utente utente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _daoUtenti.InsertAsync(utente);
                    _logger.LogInformation("Utente creato con successo: {Email}", utente.Email);
                    TempData["Success"] = "Utente creato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione dell'utente: {Email}", utente.Email);
                    TempData["Error"] = "Errore nella creazione dell'utente";
                    return View(utente);
                }
            }
            return View(utente);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var utente = await _daoUtenti.GetByIdAsync(id);
                if (utente == null)
                {
                    _logger.LogWarning("Utente non trovato: {Id}", id);
                    return NotFound();
                }
                return View(utente);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'utente: {Id}", id);
                return HandleException(ex);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Nome,Cognome,Email,PasswordHash,Ruolo,Team")] Utente utente)
        {
            if (id != utente.ID)
            {
                _logger.LogWarning("ID utente non corrispondente");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _daoUtenti.GetByIdAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    if (string.IsNullOrEmpty(utente.PasswordHash))
                    {
                        utente.PasswordHash = existingUser.PasswordHash;
                    }

                    await _daoUtenti.UpdateAsync(utente);
                    _logger.LogInformation("Utente aggiornato con successo: {Id}", id);
                    TempData["Success"] = "Utente modificato con successo";
                    return RedirectToAction(nameof(Index));
                }
                catch (DAOException ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento dell'utente: {Id}", id);
                    TempData["Error"] = "Errore durante la modifica dell'utente";
                    return View(utente);
                }
            }
            return View(utente);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var utente = await _daoUtenti.GetByIdAsync(id);
                if (utente == null)
                {
                    _logger.LogWarning("Utente non trovato per l'eliminazione: {Id}", id);
                    return NotFound();
                }

                if (utente.Ruolo == "Admin" && !await HasOtherAdminsAsync(id))
                {
                    TempData["Error"] = "Impossibile eliminare l'ultimo amministratore";
                    return RedirectToAction(nameof(Index));
                }

                return View(utente);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'utente per l'eliminazione: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var utente = await _daoUtenti.GetByIdAsync(id);
                if (utente == null)
                {
                    return NotFound();
                }

                if (utente.Ruolo == "Admin" && !await HasOtherAdminsAsync(id))
                {
                    TempData["Error"] = "Impossibile eliminare l'ultimo amministratore";
                    return RedirectToAction(nameof(Index));
                }

                await _daoUtenti.DeleteAsync(id);
                _logger.LogInformation("Utente eliminato con successo: {Id}", id);
                TempData["Success"] = "Utente eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione dell'utente: {Id}", id);
                TempData["Error"] = "Errore durante l'eliminazione dell'utente";
                return RedirectToAction(nameof(Index));
            }
        }
        private async Task<bool> HasOtherAdminsAsync(int currentUserId)
        {
            try
            {
                var allUsers = await _daoUtenti.GetAllAsync();
                return allUsers.Any(u => u.Ruolo == "Admin" && u.ID != currentUserId);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Errore durante la verifica di altri amministratori");
                return true; // Safe default
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _daoUtenti.AuthenticateAsync(model.Username, model.Password);

                    if (user != null)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Ruolo),
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    new Claim("UserId", user.ID.ToString())
                };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = model.RememberMe ?
                                DateTimeOffset.UtcNow.AddDays(30) :
                                DateTimeOffset.UtcNow.AddHours(8)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            authProperties);

                        _logger.LogInformation("Login effettuato con successo per l'utente: {Email}", user.Email);
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", "Username o password non validi");
                }
                catch (DAOException ex)
                {
                    _logger.LogError(ex, "Errore durante l'autenticazione");
                    ModelState.AddModelError("", "Errore durante l'autenticazione. Riprova.");
                }
            }
            return View(model);
        }

        [HttpPost]
            [ValidateAntiForgeryToken]
            [AllowAnonymous]
            public async Task<IActionResult> Logout()
            {
                _logger.LogInformation("Logout eseguito per l'utente: {User}", User.Identity?.Name);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] FotoProfiloViewModel model)
        {
            try
            {
                if (model.Foto == null || model.Foto.Length == 0)
                {
                    return Json(new { success = false, message = "Nessun file caricato" });
                }

                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(model.Foto.ContentType.ToLower()))
                {
                    return Json(new { success = false, message = "Formato file non supportato" });
                }

                if (model.Foto.Length > 2 * 1024 * 1024) // 2MB limit
                {
                    return Json(new { success = false, message = "L'immagine non può superare i 2MB" });
                }

                using var memoryStream = new MemoryStream();
                await model.Foto.CopyToAsync(memoryStream);
                await _daoUtenti.UpdateProfilePictureAsync(model.UtenteId, memoryStream.ToArray());

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile picture for user: {Id}", model.UtenteId);
                return Json(new { success = false, message = "Errore durante l'aggiornamento" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProfilePicture(int id)
        {
            try
            {
                await _daoUtenti.SetDefaultProfilePictureAsync(id);
                return RedirectToAction("Index", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default profile picture for user: {Id}", id);
                return RedirectToAction("Index", "Profile");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetProfilePicture(int id)
        {
            try
            {
                var utente = await _daoUtenti.GetByIdAsync(id);
                if (utente?.FotoProfilo != null)
                {
                    return File(utente.FotoProfilo, "image/jpeg");
                }

                var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "images", "profile.jpg");
                return PhysicalFile(defaultAvatarPath, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile picture for user: {Id}", id);
                var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "images", "profile.jpg");
                return PhysicalFile(defaultAvatarPath, "image/jpeg");
            }
        }

    }
}
