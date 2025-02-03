using InDaCompany.Data.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InDaCompany.Models;

namespace InDaCompany.Controllers;

[Authorize]
public class ForumController : BaseController
{
    private readonly DAOForum _daoForum;

    public ForumController(IConfiguration configuration) : base(configuration)
    {
        _daoForum = new DAOForum(ConnectionString);
    }
    public ActionResult Index()
    {
        var prodotti = _daoForum.GetAll();
        return View(prodotti);
    }
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(Forum forum)
    {
        if (ModelState.IsValid)
        {
            _daoForum.Insert(forum);
            return RedirectToAction(nameof(Index));
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
    public ActionResult Edit(Forum forum)
    {
        if (ModelState.IsValid)
        {
            _daoForum.Update(forum);
            return RedirectToAction(nameof(Index));
        }
        return View(forum);
    }

    public ActionResult Delete(int id)
    {
        var forum = _daoForum.GetById(id);
        if (forum == null) return NotFound();
        return View(forum);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        _daoForum.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    //inserire metodo per creare nuovo thread all'interno del forum

}
