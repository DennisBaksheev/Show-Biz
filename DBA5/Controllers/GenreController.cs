using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBA5.Controllers
{
    public class GenreController : Controller
    {
        // Reference to the data manager
        private Manager m = new Manager();

        // GET: Genre
        public ActionResult Index()
        {
            return View(m.GenreGetAll());
        }
    }
}
