using InDaCompany.Data.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InDaCompany.Models;

namespace InDaCompany.Controllers;

[Authorize]
public class ForumController : BaseController
{
    private readonly IDAOForum _daoForum;

    public ForumController(IConfiguration configuration, IDAOForum daoForum) : base(configuration)
    {
        _daoForum = daoForum;
    }
    public ActionResult Index()
    {
        var prodotti = _daoForum.GetAll();
        return View(forums);
    }
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Forum forum)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _daoForum.Insert(forum);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again.");
            }
        }
        return View(forum);
    }

    public ActionResult Edit(int id)
    {
        var forum = _daoForum.GetById(id);
        if (forum == null) return NotFound();
        return View(forum);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Forum forum)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _daoForum.Update(forum);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again.");
            }
        }
        return View(forum);
    }

    public ActionResult Delete(int id)
    {
        var forum = _daoForum.GetById(id);
        if (forum == null) return NotFound();
        return View(forum);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        try
        {
            _daoForum.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    //inserire metodo per creare nuovo thread all'interno del forum

}
