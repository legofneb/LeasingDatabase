using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class VendorRateController : Controller
    {
        //
        // GET: /VendorRate/

        public ActionResult Index()
        {
			var gridModel = new LeasingDatabase.Models.Grid.VendorRateJqGridModel();
			var ordersGrid = gridModel.OrdersGrid;

			SetUpGrid(ordersGrid);

			return View(gridModel);
        }

		public JsonResult DataRequested()
		{
			var gridModel = new LeasingDatabase.Models.Grid.VendorRateJqGridModel();
			var db = new AuleaseEntities();

			SetUpGrid(gridModel.OrdersGrid);

			List<VendorRate> rates = new List<VendorRate>();

			var Types = db.VendorRates.Select(n => new { n.TypeId, n.Term }).Distinct();

			foreach (var type in Types)
			{
				rates.Add(db.VendorRates.Where(n => n.TypeId == type.TypeId && n.Term == type.Term).OrderByDescending(n => n.BeginDate).FirstOrDefault());
			}

			var model = rates.Select(n => new VendorRateModel
			{
				Id = n.Id,
				Rate = n.Rate,
				RateType = n.Type.Name,
				Term = n.Term
			}).AsQueryable();

			return gridModel.OrdersGrid.DataBind(model);
		}

		public void EditRows()
		{
			var gridModel = new LeasingDatabase.Models.Grid.OrdersJqGridModel();
			var db = new AuleaseEntities();

			var e = gridModel.OrdersGrid.GetEditData(); // Edit Row

			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
			{
				int Term = Convert.ToInt32(e.RowData["Term"]);
				string Type = e.RowData["RateType"].ToString();
				decimal Rate = Convert.ToDecimal(e.RowData["Rate"]);

				VendorRate lastRate = db.VendorRates.Where(n => n.Term == Term && n.Type.Name == Type).OrderByDescending(n => n.BeginDate).First();

                VendorRate newRate = new VendorRate();
                newRate.Type = lastRate.Type;
                newRate.Term = lastRate.Term;
                newRate.BeginDate = DateTime.Now;
                newRate.Rate = Rate;

                db.VendorRates.Add(newRate);

                db.SaveChanges();
			}
			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
			{
				int Term = Convert.ToInt32(e.RowData["Term"]);
				string Type = e.RowData["RateType"].ToString();
				decimal Rate = Convert.ToDecimal(e.RowData["Rate"]);

				VendorRate newRate = new VendorRate();
				newRate.Type = db.Types.Where(n => n.Name == Type).Single();
				newRate.Term = Term;
				newRate.BeginDate = DateTime.Now;
				newRate.Rate = Rate;

				db.VendorRates.Add(newRate);
				db.SaveChanges();
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
			ordersGrid.ID = "VendorRateGrid";
			// Setting the DataUrl to an action (method) in the controller is required.
			// This action will return the data needed by the grid
			ordersGrid.DataUrl = Url.Action("DataRequested");
			ordersGrid.EditUrl = Url.Action("EditRows");
		}

    }
}
