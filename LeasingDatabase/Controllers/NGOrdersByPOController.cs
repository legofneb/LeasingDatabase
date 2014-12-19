using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class NGOrdersByPOController : Controller
    {
        //
        // GET: /NGOrdersBySR/

        public ActionResult Index()
        {
            return View();
        }
        
        public PartialViewResult Home()
        {
            return PartialView();
        }

        public PartialViewResult System()
        {
            return PartialView();
        }

        public PartialViewResult Billing()
        {
            return PartialView();
        }
    }
}
