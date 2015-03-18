using aulease.Entities;
using CWSToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
	[AuthorizeUser("Admin", "Users")]
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return RedirectToAction("Index", "NG");

			AuleaseEntities db = new AuleaseEntities();
            ViewData["NewOrders"] = db.SystemGroups.Where(n => n.PO == null).Select(n => n.Order).ToList();
			ViewData["Repairs"] = db.Repairs.Where(n => n.Status.Id != Status.CompletedRepair).Count();
            ViewData["Tasks"] = db.Tasks.Where(n => n.Status.Id != Status.CompletedTask).ToList();

            //List<string[]> NewOrderList = new List<string[]>();

            //List<SystemGroup> list = db.SystemGroups.Where(n => n.PO == null).OrderByDescending(n => n.Id).Take(10).OrderBy(n => n.Id).ToList();
            //list.Reverse();
            //foreach (var item in list)
            //{
            //    string[] array = new string[6];
            //    array[0] = @Url.Action("Index", "Orders");
            //    array[1] = item.Order.User.GID + " has placed an order. And sometime later I will add more details about this specific order.";
            //    array[2] = item.Order.Date.ToString("MMM");
            //    array[3] = item.Order.Date.Day.ToString();
            //    array[4] = item.Order.User.GID;
            //    NewOrderList.Add(array);
            //}

            //ViewData["NewOrdersList"] = NewOrderList;

            return View(/*ViewData*/);
        }

    }
}
