using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class EOLController : Controller
    {
        //
        // GET: /EOL/

        public ActionResult Index()
        {
			return View();
        }

		public ActionResult Report(string date)
		{
			Session["date"] = date;

			var gridModel = new LeasingDatabase.Models.Grid.EOLJqGridModel();
			var ordersGrid = gridModel.OrdersGrid;

			SetUpGrid(ordersGrid);

			return View(gridModel);
		}

		public JsonResult DataRequested()
		{
			string DateString = Session["date"].ToString();
			int Month = Convert.ToInt32(DateString.Substring(0, 2));
			int Year = Convert.ToInt32(DateString.Substring(2, 4));
			//string Parsed = DateString.Substring(0, 2) + "/" + DateString.Substring(2, 4);
			//DateTime date = Convert.ToDateTime(DateString);

			var gridModel = new LeasingDatabase.Models.Grid.EOLJqGridModel();
			var db = new AuleaseEntities();

			SetUpGrid(gridModel.OrdersGrid);

			var EOL = db.Components.Where(n => n.ReturnDate.Value.Month == Month && n.ReturnDate.Value.Year == Year);

			var model = EOL.Select(n => new EOLModel
			{
				Id = n.Id,
                SN = n.SerialNumber.Substring(n.SerialNumber.Length - 7,n.SerialNumber.Length),
				SerialNumber = n.SerialNumber,
				ComponentType = n.Type.Name,
				Make = n.Make.Name,
				Model = n.Model.Name ?? null,
                StatementName = n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().StatementName,
				GID = n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.User.GID,
                EndBillingDate = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate,
                ReturnDate = n.ReturnDate,
				Damages = n.Damages
			});

			return gridModel.OrdersGrid.DataBind(model);
		}

		public void SetUpGrid(Trirand.Web.Mvc.JQGrid ordersGrid)
		{
			// Customize/change some of the default settings for this model
			// ID is a mandatory field. Must by unique if you have several grids on one page.
			ordersGrid.ID = "EOLGrid";
			// Setting the DataUrl to an action (method) in the controller is required.
			// This action will return the data needed by the grid
			ordersGrid.DataUrl = Url.Action("DataRequested");
			ordersGrid.EditUrl = Url.Action("EditRows");
		}

        public ActionResult ExportToExcel()
        {

            var gridModel = new LeasingDatabase.Models.Grid.EOLDetailedJqGridModel();
            var ordersGrid = gridModel.OrdersGrid;
            AuleaseEntities db = new AuleaseEntities();
            
            SetUpGrid(gridModel.OrdersGrid);

            string DateString = Session["date"].ToString();
            int Month = Convert.ToInt32(DateString.Substring(0, 2));
            int Year = Convert.ToInt32(DateString.Substring(2, 4));

			var EOL = db.Components.Where(n => n.ReturnDate.Value.Month == Month && n.ReturnDate.Value.Year == Year);

            var model = EOL.Select(n => new EOLModel
			{
                Id = n.Id,
                SN = n.SerialNumber.Substring(n.SerialNumber.Length - 7,n.SerialNumber.Length),
                LeaseTag = n.LeaseTag,
                SR = n.Leases.FirstOrDefault().SystemGroup.PO.PONumber,
                SerialNumber = n.SerialNumber,
                ComponentType = n.Type.Name,
                Make = n.Make.Name,
                Model = n.Model.Name ?? null,
                DepartmentName = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Name,
                Term = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Overhead.Term,
                MonthlyCharge = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().MonthlyCharge.Value,
                FOP = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Fund + "-" + n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Org + "-" + n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Program,
                StatementName = n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().StatementName,
                GID = n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.User.GID,
                EndBillingDate = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate,
                ReturnDate = n.ReturnDate,
                Damages = n.Damages
			}).AsQueryable();
           
            ordersGrid.ExportToExcel(model, "grid.xls");
      

            return View();
        }

    }
}
