using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class NewSRsController : Controller
    {
        //
        // GET: /NewSRs/

        public ActionResult Index()
        {
            AuleaseEntities db = new AuleaseEntities();

            List<PO> SRs = db.POes.Where(n => n.SystemGroups.Count > 0).Where(n => !n.SystemGroups.Any(o => o.Leases.Any(p => p.MonthlyCharge != null))).OrderByDescending(n => n.SystemGroups.FirstOrDefault().Order.Date).ToList();

            ViewBag.SRs = SRs;

            return View();
        }

        public PartialViewResult SRDetails(int id)
        {
            AuleaseEntities db = new AuleaseEntities();

            PO SR = db.POes.Where(n => n.Id == id).Single();

            ViewBag.SR = SR;

            return PartialView();
        }

    }
}
