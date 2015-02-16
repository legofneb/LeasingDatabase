using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class NGComponentsController : Controller
    {
        //
        // GET: /NGComponents/

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Home()
        {
            return PartialView();
        }

        public PartialViewResult Component()
        {
            return PartialView();
        }
    }
}
