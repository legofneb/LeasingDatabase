using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class NGBillingCoordinatorController : Controller
    {
        //
        // GET: /NGBillingCoordinator/

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult FOPView()
        {
            return PartialView();
        }

        public PartialViewResult GIDView()
        {
            return PartialView();
        }
    }
}
