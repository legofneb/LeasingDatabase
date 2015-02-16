using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class NGBillingReportController : Controller
    {
        //
        // GET: /NGBillingReport/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SimpleBilling()
        {
            return View();
        }

        public ActionResult DetailedBilling()
        {
            return View();
        }
    }
}
