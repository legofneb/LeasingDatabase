using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class NGReportsController : Controller
    {
        //
        // GET: /NGReports/

        public ActionResult TSMReport()
        {
            return View();
        }

        public ActionResult ReconcileAgainstInvoice()
        {
            return View();
        }

    }
}
