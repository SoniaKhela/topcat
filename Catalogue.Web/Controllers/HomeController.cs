using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Catalogue.Data.Repository;

namespace Catalogue.Web.Controllers.Home
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return RedirectPermanent("~/app");

        }

        
    }
}
