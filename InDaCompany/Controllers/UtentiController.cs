using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;


namespace InDaCompany.Controllers;
[Authorize]
public class UtentiController : Controller
{
    private readonly IDAOUtenti _daoUtenti;
    private readonly IConfiguration _configuration;


    public UtentiController(IConfiguration configuration, IDAOUtenti daoUtenti)
    {
        _daoUtenti = daoUtenti;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        var utenti = _daoUtenti.GetAll();
        return View(utenti);
    }

    public IActionResult Create()
    {
        return View(new Utente());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Utente utente)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _daoUtenti.Insert(utente);
                TempData["Success"] = "Utente creato con successo!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore nella creazione del thread: {ex.Message}";
                return View(utente);
            }
        }

        return View(utente);
    }
    public IActionResult Edit(int id)
    {
        var utente = _daoUtenti.GetById(id);
        if (utente == null)
        {
            return NotFound();
        }
        return View(utente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Utente utente)
    {
        if (id != utente.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _daoUtenti.Update(utente);
                TempData["Success"] = "Utente modificato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore durante la modifica dell'utente: {ex.Message}";
                return View(utente);
            }
        }
        return View(utente);
    }

    public IActionResult Delete(int id)
    {
        var utente = _daoUtenti.GetById(id);
        if (utente == null)
        {
            return NotFound();
        }
        return View(utente);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _daoUtenti.Delete(id);
            TempData["Success"] = "Utente cancellato con successo";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Errore durante la cancellazione dell'utente: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        try
        {
            var user = await _daoUtenti.GetByEmail(email);
            if (user == null)
            {
                TempData["Error"] = "Credenziali non valide";
                return RedirectToAction("Login");
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                TempData["Error"] = "Credenziali non valide";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Ruolo),
                new Claim("UserId", user.ID.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["Success"] = $"Welcome {user.Nome} {user.Cognome}";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "C'è stato un errore durante il login";
            return RedirectToAction("Login");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
