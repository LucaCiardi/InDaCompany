using InDaCompany.Data.Implementations;
using InDaCompany.Models;
using Microsoft.AspNetCore.Mvc;

namespace InDaCompany.Controllers;

public class PostController : Controller
{
    private readonly DAOPost _daoPost;

    public PostController(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        _daoPost = new DAOPost(connectionString);
    }

    public IActionResult Index()
    {
        var post = _daoPost.GetAll();
        return View(post);
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
                _daoPost.Insert(post);
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating product: {ex.Message}";
            }
        }

        ViewBag.Negozi = _daoPost.GetAll();
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
                _daoPost.Update(post);
                TempData["Success"] = "Product updated successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating product: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
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
            _daoPost.Delete(id);
            TempData["Success"] = "Product deleted successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting product: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
