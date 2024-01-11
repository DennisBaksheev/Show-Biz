using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBA5.Models; // Assuming your models are in this namespace

namespace DBA5.Controllers
{
   public class EpisodeController : Controller
   {
      private Manager m = new Manager();

      // GET: Episode
      public ActionResult Index()
      {
         return View(m.EpisodeGetAll());
      }

      // GET: Episode/Details/5
      public ActionResult Details(int? id)
      {
         var theEpisode = m.EpisodeGetByIdWithShowName(id.GetValueOrDefault());
         if (theEpisode == null)
         {
            return HttpNotFound();
         }
         return View(theEpisode);
      }

      // GET: Episodes/Video/5
      [Route("Episodes/Video/{id}")]
      public ActionResult ShowVideo(int? id)
      {
         var o = m.EpisodeVideoGetById(id.GetValueOrDefault());
         if (o == null)
         {
            return HttpNotFound();
         }
         return File(o.Video, o.VideoContentType);
      }

      // GET: Episode/Edit/5
      [Authorize(Roles = "Admin, Executive, Coordinator")]
      public ActionResult Edit(int? id)
      {
         var theEpisode = m.EpisodeGetByIdWithShowName(id.GetValueOrDefault());
         if (theEpisode == null)
         {
            return HttpNotFound();
         }

         var editModel = new EpisodeEditViewModel
         {
            Id = theEpisode.Id,
            Name = theEpisode.Name,
            SeasonNumber = theEpisode.SeasonNumber,
            EpisodeNumber = theEpisode.EpisodeNumber,
            Genre = theEpisode.Genre,
            AirDate = theEpisode.AirDate,
            ImageUrl = theEpisode.ImageUrl
         };
         return View(editModel);
      }

      // POST: Episode/Edit/5
      [HttpPost]
      [Authorize(Roles = "Admin, Executive, Coordinator")]
      [ValidateAntiForgeryToken]
      public ActionResult Edit(int id, EpisodeEditViewModel editedEpisode)
      {
         if (!ModelState.IsValid)
         {
            return View(editedEpisode);
         }

         if (!m.UpdateEpisode(editedEpisode))
         {
            ModelState.AddModelError("", "Unable to update the episode");
            return View(editedEpisode);
         }

         return RedirectToAction("Index");
      }

      // GET: Episode/Delete/5
      [Authorize(Roles = "Admin")]
      public ActionResult Delete(int? id)
      {
         var theEpisode = m.EpisodeGetByIdWithShowName(id.GetValueOrDefault());
         if (theEpisode == null)
         {
            return HttpNotFound();
         }
         return View(theEpisode);
      }

      // POST: Episode/Delete/5
      [HttpPost, ActionName("Delete")]
      [Authorize(Roles = "Admin")]
      [ValidateAntiForgeryToken]
      public ActionResult DeleteConfirmed(int id)
      {
         m.DeleteEpisode(id);
         return RedirectToAction("Index");
      }
   }
}
