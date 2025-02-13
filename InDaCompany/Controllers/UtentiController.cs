using System.Security.Claims;
using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using InDaCompany.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UtentiController : BaseController
    {
        private readonly IDAOUtenti _daoUtenti;
        private readonly ILogger<UtentiController> _logger;

        public UtentiController(
            IConfiguration configuration,
            IDAOUtenti daoUtenti,
            ILogger<UtentiController> logger) : base(configuration, logger)
        {
            _daoUtenti = daoUtenti;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var utenti = await _daoUtenti.GetAllAsync();
                return View(utenti);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return HandleException(ex);
            }
        }

        public IActionResult Create() => View(new Utente());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Utente utente)
        {
            if (!ModelState.IsValid) return View(utente);

            try
            {
                await _daoUtenti.InsertAsync(utente);
                _logger.LogInformation("User created successfully: {Email}", utente.Email);
                TempData["Success"] = "Utente creato con successo!";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Error creating user: {Email}", utente.Email);
                TempData["Error"] = "Errore nella creazione dell'utente";
                return View(utente);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var utente = await _daoUtenti.GetByIdAsync(id);
                if (utente == null)
                {
                    _logger.LogWarning("User not found: {Id}", id);
                    return NotFound();
                }
                return View(utente);
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Error retrieving user for edit: {Id}", id);
                return HandleException(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Utente utente)
        {
            if (id != utente.ID) return NotFound();
            if (!ModelState.IsValid) return View(utente);

            try
            {
                var existingUser = await _daoUtenti.GetByIdAsync(id);
                if (existingUser == null) return NotFound();

                if (string.IsNullOrEmpty(utente.PasswordHash))
                {
                    utente.PasswordHash = existingUser.PasswordHash;
                }

                await _daoUtenti.UpdateAsync(utente);
                _logger.LogInformation("User updated successfully: {Id}", id);
                TempData["Success"] = "Utente modificato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (DAOException ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", id);
                TempData["Error"] = "Errore durante la modifica dell'utente";
                return View(utente);
            }
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

                if (model.Foto.Length > 2 * 1024 * 1024)
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

        [AllowAnonymous]
        public async Task<IActionResult> Login()
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
            if (!ModelState.IsValid) return View(model);

            try
            {
                var user = await _daoUtenti.AuthenticateAsync(model.Username, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Username o password incorretti");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Email),
                    new(ClaimTypes.Role, user.Ruolo),
                    new(ClaimTypes.NameIdentifier, user.ID.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ?
                        DateTimeOffset.UtcNow.AddDays(30) :
                        DateTimeOffset.UtcNow.AddHours(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {Username}", model.Username);
                ModelState.AddModelError("", "Si è verificato un errore durante il login");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout eseguito per l'utente {User}", User.Identity?.Name);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task<bool> HasOtherAdminsAsync(int currentUserId)
        {
            try
            {
                var allUsers = await _daoUtenti.GetAllAsync();
                return allUsers.Any(u => u.Ruolo == "Admin" && u.ID != currentUserId);
            }
            catch (DAOException)
            {
                return true;
            }
        }
    }
}
