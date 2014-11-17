using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class BillingCoordinatorController : Controller
    {
        //
        // GET: /BillingCoordinator/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult FOP(string Fund, string Org, string Program)
		{
			Session["Fund"] = Fund;
			Session["Org"] = Org;
			Session["Program"] = Program;

            AuleaseEntities db = new AuleaseEntities();
            int count = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Count();
            if (count < 1)
            {
                ViewBag.error = "No Department Found";
                return RedirectToAction("Index");
            }
            else if (count > 1)
            {
                ViewBag.error = "Duplicate Departments found...this is a serious issue that needs to be resolved right away";
                return RedirectToAction("Index");
            }

			var gridModel = new LeasingDatabase.Models.Grid.BillingCoordinatorJqGridModel();
			var ordersGrid = gridModel.OrdersGrid;

			SetUpGrid(ordersGrid);

			return View(gridModel);
		}

		public JsonResult DataRequested()
		{
			string Fund = Session["Fund"].ToString();
			string Org = Session["Org"].ToString();
			string Program = Session["Program"].ToString();

			var gridModel = new LeasingDatabase.Models.Grid.BillingCoordinatorJqGridModel();
			var db = new AuleaseEntities();

			SetUpGrid(gridModel.OrdersGrid);

			var dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
			var users = dept.Coordinators;

			var model = users.Select(n => new BillingCoordinatorModel
			{
				Id = n.Id.ToString(),
				GID = n.GID,
				Fund = Fund,
                BillingAccess = n.BillingAccess,
				Org = Org,
				Program = Program
			}).AsQueryable();

			return gridModel.OrdersGrid.DataBind(model);
		}

		public void EditRows()
		{
			string Fund = Session["Fund"].ToString();
			string Org = Session["Org"].ToString();
			string Program = Session["Program"].ToString();

			var gridModel = new LeasingDatabase.Models.Grid.BillingCoordinatorJqGridModel();
			var db = new AuleaseEntities();

			var e = gridModel.OrdersGrid.GetEditData(); // Edit Row

			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
			{
				// Not gonna allow editing, just adding and deleting
				throw new NotImplementedException();
			}
			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
			{
				string GID = e.RowData["GID"];
                bool BillingAccess = Convert.ToBoolean(e.RowData["BillingAccess"]);
				Coordinator user;

				if (db.Coordinators.Any(n => n.GID == GID))
				{
					user = db.Coordinators.Where(n => n.GID == GID).Single();
				}
				else
				{
					user = new Coordinator();
                    user.BillingAccess = BillingAccess;
					user.GID = GID;
				}
                Department dept;
                if (!db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
                {
                    Department newDept = new Department();
                    newDept.Fund = Fund;
                    newDept.Org = Org;
                    newDept.Program = Program;
                    newDept.Name = "New Department";
                    db.Departments.Add(newDept);
                    db.SaveChanges();
                }

                dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();

				dept.Coordinators.Add(user);
				db.SaveChanges();
			}
			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.DeleteRow)
			{
				int Id = Convert.ToInt32(e.RowData["Id"]);
				var dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
				Coordinator user = db.Coordinators.Where(n => n.Id == Id).Single();

				dept.Coordinators.Remove(user);
                db.SaveChanges();
			}
		}

		public void SetUpGrid(Trirand.Web.Mvc.JQGrid ordersGrid)
		{
			// Customize/change some of the default settings for this model
			// ID is a mandatory field. Must by unique if you have several grids on one page.
			ordersGrid.ID = "BillingCoordinatorGrid";
			// Setting the DataUrl to an action (method) in the controller is required.
			// This action will return the data needed by the grid
			ordersGrid.DataUrl = Url.Action("DataRequested");
			ordersGrid.EditUrl = Url.Action("EditRows");
		}

        /*
         * Above is JQGrid for BillingCoordinators to a FOP, Below is JQGrid for FOPs to a BillingCoordinator
         */

        public ActionResult GID(string GID)
        {
            Session["GID"] = GID;

            var gridModel = new LeasingDatabase.Models.Grid.BillingCoordinatorJqGridModel();
            var ordersGrid = gridModel.OrdersGrid;

            GIDSetUpGrid(ordersGrid);

            return View(gridModel);
        }

        public JsonResult GIDDataRequested()
        {
            string GID = Session["GID"].ToString();

            var gridModel = new LeasingDatabase.Models.Grid.BillingCoordinatorJqGridModel();
            var db = new AuleaseEntities();

            SetUpGrid(gridModel.OrdersGrid);
            bool BillingAccess = (db.Coordinators.Where(n => n.GID== GID).Count() > 0) ? db.Coordinators.Where(n => n.GID== GID).Single().BillingAccess : false;
            var dept = db.Coordinators.Where(n => n.GID == GID).SelectMany(n => n.Departments);

            var model = dept.Select(n => new BillingCoordinatorModel
            {
                Id = GID + "-" + n.Fund + "-" + n.Org + "-" + n.Program,
                GID = GID,
                Fund = n.Fund,
                Org = n.Org,
                Program = n.Program,
                BillingAccess = BillingAccess
            }).AsQueryable();

            return gridModel.OrdersGrid.DataBind(model);
        }

        public void GIDEditRows()
        {

            var gridModel = new LeasingDatabase.Models.Grid.BillingCoordinatorJqGridModel();
            var db = new AuleaseEntities();

            var e = gridModel.OrdersGrid.GetEditData(); // Edit Row

            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
            {
                // Not gonna allow editing, just adding and deleting
                throw new NotImplementedException();
            }
            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
            {
                string Fund = e.RowData["Fund"].ToString().Trim();
                string Org = e.RowData["Org"].ToString().Trim();
                string Program = e.RowData["Program"].ToString().Trim();
                string GID = e.RowData["GID"].ToString().Trim();
                Session["GID"] = GID;
                bool BillingAccess = Convert.ToBoolean(e.RowData["BillingAccess"]);

                if (!db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
                {
                    Department newDept = new Department();
                    newDept.Fund = Fund;
                    newDept.Org = Org;
                    newDept.Program = Program;
                    newDept.Name = "New Department";
                    db.Departments.Add(newDept);
                    db.SaveChanges();
                }

                if (!db.Coordinators.Any(n => n.GID == GID))
                {
                    Coordinator newCoordinator = new Coordinator();
                    newCoordinator.BillingAccess = BillingAccess;
                    newCoordinator.GID = GID;
                    db.Coordinators.Add(newCoordinator);
                    db.SaveChanges();
                }

                Coordinator user = db.Coordinators.Where(n => n.GID == GID).Single();

                var dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();

                dept.Coordinators.Add(user);
                db.SaveChanges();
            }
            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.DeleteRow)
            {
                string[] array = e.RowData["Id"].ToString().Split('-');
                string GID = array[0];
                Session["GID"] = GID;
                string Fund = array[1];
                string Org = array[2];
                string Program = array[3];

                var dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
                Coordinator user = db.Coordinators.Where(n => n.GID == GID).Single();

                dept.Coordinators.Remove(user);
                db.SaveChanges();
            }
        }

        public void GIDSetUpGrid(Trirand.Web.Mvc.JQGrid ordersGrid)
        {
            // Customize/change some of the default settings for this model
            // ID is a mandatory field. Must by unique if you have several grids on one page.
            ordersGrid.ID = "CoordinatorGrid";
            // Setting the DataUrl to an action (method) in the controller is required.
            // This action will return the data needed by the grid
            ordersGrid.DataUrl = Url.Action("GIDDataRequested");
            ordersGrid.EditUrl = Url.Action("GIDEditRows");
        }
    }
}
