using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CWSToolkit;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Users", "Admin")]
    public class NGScanController : Controller
    {
        //
        // GET: /NGScan/

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Home()
        {
            return PartialView();
        }

        public PartialViewResult Scanning()
        {
            return PartialView();
        }

        public PartialViewResult Summary()
        {
            return PartialView();
        }

        public PartialViewResult Loading()
        {
            return PartialView();
        }
    }
}
