using InDaCompany.Data.Implementations;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers;
[Authorize]
public class PostController : Controller
{
    private readonly IDAOPost _daoPost;

    public PostController(IConfiguration configuration, IDAOPost daoPost)
    {
        _daoPost = daoPost;
    }

    public IActionResult Index()
    {
        var post = _daoPost.GetAll();
        return View(posts);
    }

    public IActionResult Create()
    {
        return View(new Post());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Post post)
    {
        if (ModelState.IsValid)
        {
            try
            {
                post.AutoreID = User.GetUserId();
                _daoPost.Insert(post);
                TempData["Success"] = "Post creato con successo!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore durante la creazione del post: {ex.Message}";
                 return View(post);
            }
        }
        return View(post);
    }
    public IActionResult Edit(int id)
    {
        var post = _daoPost.GetById(id);
        if (post == null)
        {
            return NotFound();
        }
        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Post post)
    {
        if (id != post.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var originalPost = _daoPost.GetById(id);
                if (originalPost == null) return NotFound();
                post.DataCreazione = originalPost.DataCreazione;
                post.AutoreID = originalPost.AutoreID;
                _daoPost.Update(post);
                TempData["Success"] = "Post modificato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Errore durante la modifica del post: {ex.Message}";
                 return View(post);
            }
        }
        return View(post);
    }

    public IActionResult Delete(int id)
    {
        var post = _daoPost.GetById(id);
        if (post == null)
        {
            return NotFound();
        }
        return View(post);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            var post = _daoPost.GetById(id);
            if (post == null) return NotFound();
            _daoPost.Delete(id);
            TempData["Success"] = "Post eliminato con successo";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Errore durante l'eliminazione del post: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
