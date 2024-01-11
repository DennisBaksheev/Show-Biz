using DBA5.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DBA5.Controllers
{
   public class ShowController : Controller
   {
      private Manager m = new Manager();

      // GET: Show
      public ActionResult Index()
      {
         return View(m.ShowGetAll());
      }

      // GET: Show/Details/5
      public ActionResult Details(int? id)
      {
         var theShow = m.ShowGetByIdWithInfo(id.GetValueOrDefault());
         if (theShow == null)
         {
            return HttpNotFound();
         }
         return View(theShow);
      }

      // GET: Shows/{id}/AddEpisode
      [Route("Shows/{id}/AddEpisode"), Authorize(Roles = "Admin, Executive, Coordinator")]
      public ActionResult AddEpisode(int id)
      {
         var show = m.ShowGetByIdWithInfo(id);
         if (show == null)
         {
            return HttpNotFound();
         }

         var formModel = new EpisodeAddFormViewModel
         {
            ShowId = show.Id,
            ShowName = show.Name,
            GenreList = new SelectList(m.GenreGetAll(), "Name", "Name")
         };

         return View(formModel);
      }

      // POST: Shows/{id}/AddEpisode
      [Route("Shows/{id}/AddEpisode"), Authorize(Roles = "Admin, Executive, Coordinator"), ValidateInput(false)]
      [HttpPost]
      public ActionResult AddEpisode(EpisodeAddViewModel newItem)
      {
         if (!ModelState.IsValid)
         {
            return View(newItem);
         }

         var addedItem = m.EpisodeAdd(newItem);
         if (addedItem == null)
         {
            return View(newItem);
         }
         return RedirectToAction("Details", "Episode", new { id = addedItem.Id });
      }

      // GET: Show/Edit/5
      [Authorize(Roles = "Admin, Executive, Coordinator")]
      public ActionResult Edit(int? id)
      {
         var show = m.ShowGetByIdWithInfo(id.GetValueOrDefault());
         if (show == null)
         {
            return HttpNotFound();
         }

         var editModel = new ShowEditViewModel
         {
            Id = show.Id,
            Name = show.Name,
            Genre = show.Genre,
            ReleaseDate = show.ReleaseDate,
            ImageUrl = show.ImageUrl,
            Coordinator = show.Coordinator,
            Premise = show.Premise
         };

         return View(editModel);
      }

      // GET: Show/Delete/5
      [Authorize(Roles = "Admin")]
      public ActionResult Delete(int? id)
      {
         var show = m.ShowGetByIdWithInfo(id.GetValueOrDefault());
         if (show == null)
         {
            return HttpNotFound();
         }
         return View(show);
      }

      // POST: Show/Delete/5
      [HttpPost, ActionName("Delete")]
      [Authorize(Roles = "Admin")]
      [ValidateAntiForgeryToken]
      public ActionResult DeleteConfirmed(int id)
      {
         if (m.DeleteShow(id))
         {
            return RedirectToAction("Index");
         }
         else
         {
            return RedirectToAction("Delete", new { id = id });
         }
      }


      // POST: Show/Edit/5
      [HttpPost]
      [Authorize(Roles = "Admin, Executive, Coordinator")]
      [ValidateAntiForgeryToken]
      public ActionResult Edit(ShowEditViewModel model)
      {
         if (!ModelState.IsValid)
         {
            return View(model);
         }

         if (m.UpdateShow(model))
         {
            return RedirectToAction("Index");
         }
         else
         {
            ModelState.AddModelError("", "Unable to update the show. Please check the details and try again.");
            return View(model);
         }
      }

      
   }
}
