using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class DepartmentController : Controller
    {
        //
        // GET: /Department/

        public ActionResult Index()
        {
			var gridModel = new LeasingDatabase.Models.Grid.DepartmentJqGridModel();
			var ordersGrid = gridModel.OrdersGrid;

			SetUpGrid(ordersGrid);

			return View(gridModel);
        }


		public JsonResult DataRequested()
		{
			var gridModel = new LeasingDatabase.Models.Grid.DepartmentJqGridModel();
			var db = new AuleaseEntities();

			SetUpGrid(gridModel.OrdersGrid);

            DateTime date = DateTime.Now.AddMonths(-3);

			var depts = db.Departments.Where(n => (n.Leases.Where(o => o.EndDate > date).Count() > 0));

			var model = depts.Select(n => new DepartmentModel
			{
				Id = n.Id,
				Name = n.Name,
				Fund = n.Fund,
				Org = n.Org,
				Program = n.Program
			});

			return gridModel.OrdersGrid.DataBind(model);
		}

		public void EditRows()
		{
			var gridModel = new LeasingDatabase.Models.Grid.DepartmentJqGridModel();
			var db = new AuleaseEntities();

			var e = gridModel.OrdersGrid.GetEditData(); // Edit Row

			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
			{
				int _Id = Convert.ToInt32(e.RowData["Id"]);
				Department dept = db.Departments.Where(n => n.Id == _Id).Single();

				string _Fund = e.RowData["Fund"].ToString();
				string _Org = e.RowData["Org"].ToString();
				string _Program = e.RowData["Program"].ToString();
				string _Name = e.RowData["Name"].ToString();

				dept.Name = _Name;
				dept.Fund = _Fund;
				dept.Org = _Org;
				dept.Program = _Program;

				db.SaveChanges();				
			}
			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
			{
				// Add
			}
			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.DeleteRow)
			{
				// Not gonna allow deleting of rates
				throw new NotImplementedException();
			}
		}

		public void SetUpGrid(Trirand.Web.Mvc.JQGrid ordersGrid)
		{
			// Customize/change some of the default settings for this model
			// ID is a mandatory field. Must by unique if you have several grids on one page.
			ordersGrid.ID = "DepartmentGrid";
			// Setting the DataUrl to an action (method) in the controller is required.
			// This action will return the data needed by the grid
			ordersGrid.DataUrl = Url.Action("DataRequested");
			ordersGrid.EditUrl = Url.Action("EditRows");
		}

    }
}
