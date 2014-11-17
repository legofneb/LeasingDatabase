using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class NewOrdersController : Controller
    {
        //
        // GET: /NewOrders/

        public ActionResult Index()
        {
            AuleaseEntities db = new AuleaseEntities();

            List<Order> Orders = db.Orders.Where(n => n.SystemGroups.Any(o => o.Leases.Any(p => p.MonthlyCharge == null)) && n.SystemGroups.Any(o => o.PO.PONumber == null)).OrderByDescending(n => n.Date).ToList();
            ViewBag.Orders = Orders;

            return View();
        }

        public PartialViewResult OrderDetails(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();

            Order order = db.Orders.Where(n => n.Id == Id).Single();

            ViewBag.Order = order;

            return PartialView();
        }

        public PartialViewResult SystemDetails(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();

            SystemGroup group = db.SystemGroups.Where(n => n.Id == Id).Single();

            ViewBag.Group = group;

            return PartialView();
        }

        public PartialViewResult ChangeConfiguration(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();

            List<Component> comps = db.Orders.Where(n => n.Id == Id).Single().GetConfiguration().ToList();

            ViewBag.comps = comps;

            return PartialView();
        }

    }
}
