using CWSToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Users", "Admin")]
    public class NGRepairController : Controller
    {
        //
        // GET: /NGRepair/

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Home()
        {
            return PartialView();
        }

        public PartialViewResult New()
        {
            return PartialView();
        }
    }
}
