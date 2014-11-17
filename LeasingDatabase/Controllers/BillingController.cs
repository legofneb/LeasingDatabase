using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class BillingController : Controller
    {
        //
        // GET: /Billing/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult SerialNumber(string SerialNumber)
		{
			Session["SN"] = SerialNumber;
			var gridModel = new LeasingDatabase.Models.Grid.BillingJqGridModel();
			var ordersGrid = gridModel.OrdersGrid;

			SetUpGrid(ordersGrid);

			return View(gridModel);
		}

		public JsonResult DataRequested()
		{
			var gridModel = new LeasingDatabase.Models.Grid.BillingJqGridModel();
			var db = new AuleaseEntities();

			string SN = Session["SN"].ToString();

			SetUpGrid(gridModel.OrdersGrid);

			var leases = db.Leases.Where(n => n.Component.SerialNumber == SN);

			var model = leases.Select(n => new BillingModel
			{
				Id = n.Id,
				BeginDate = n.BeginDate,
				EndDate = n.EndDate,
				StatementName = n.StatementName,
				Timestamp = n.Timestamp,
				ContractNumber = n.ContractNumber,
				Fund = n.Department.Fund,
				Org = n.Department.Org,
				Program = n.Department.Program,
				RateLevel = n.Overhead.RateLevel,
				MonthlyCharge = n.MonthlyCharge.HasValue ? n.MonthlyCharge.Value : 0M,//n.MonthlyCharge??0M,
				Term = n.Overhead.Term,
                PrimaryCharge = (n.Charges.Where(o => o.Type.Name == "CPU" || o.Type.Name == "Laptop" || o.Type.Name == "Server" || o.Type.Name == "Monitor").Count() > 0) ? n.Charges.Where(o => o.Type.Name == "CPU" || o.Type.Name == "Laptop" || o.Type.Name == "Server" || o.Type.Name == "Monitor").Where(p => p.Price != null).Sum(o => o.Price) : 0M,
				SecondaryCharges = (n.Charges.Where(o => o.Type.Name != "CPU" && o.Type.Name != "Laptop" && o.Type.Name != "Server" && o.Type.Name != "Monitor").Count() > 0) ? n.Charges.Where(o => o.Type.Name != "CPU" && o.Type.Name != "Laptop" && o.Type.Name != "Server" && o.Type.Name != "Monitor").Where(p => p.Price != null).Sum(o => o.Price) : 0M
			});

			return gridModel.OrdersGrid.DataBind(model);
		}

        public ActionResult EditRows()
        {
			var gridModel = new LeasingDatabase.Models.Grid.OrdersJqGridModel();
			var db = new AuleaseEntities();

			var e = gridModel.OrdersGrid.GetEditData(); // Edit Row

			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
			{
                int id = Convert.ToInt32(e.RowData["Id"].ToString());
                Lease lease = db.Leases.Where(n => n.Id == id).Single();

                string validationMessage = ValidationEditRow(e);

                if (!String.IsNullOrWhiteSpace(validationMessage))
                { return gridModel.OrdersGrid.ShowEditValidationMessage(validationMessage); }

                string _Fund = e.RowData["Fund"].ToString().Trim();
                string _Org = e.RowData["Org"].ToString().Trim();
                string _Program = e.RowData["Program"].ToString().Trim();

                string _ContractNumber = e.RowData["ContractNumber"].ToString().Trim();
                DateTime _BeginDate = Convert.ToDateTime(e.RowData["BeginDate"].ToString());
                DateTime _EndDate = Convert.ToDateTime(e.RowData["EndDate"].ToString());
                string _StatementName = e.RowData["StatementName"].ToString();

                decimal _MonthlyCharge = Convert.ToDecimal(e.RowData["MonthlyCharge"].ToString());


                Department dept = db.Departments.Where(n => n.Fund == _Fund && n.Org == _Org && n.Program == _Program).Single();
                lease.Department = dept;
                lease.BeginDate = _BeginDate;
                lease.EndDate = _EndDate;
                lease.StatementName = _StatementName;
                lease.ContractNumber = _ContractNumber;
                lease.MonthlyCharge = _MonthlyCharge;

                db.SaveChanges();
                
			}
			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
			{
                // Add Row
			}
			if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.DeleteRow)
			{
                int id = Convert.ToInt32(e.RowData["Id"].ToString());
                Lease lease = db.Leases.Where(n => n.Id == id).Single();

                Session["SN"] = lease.Component.SerialNumber;

                foreach (var charge in lease.Charges)
                {
                    charge.Leases.Remove(lease);
                }

                Component comp = lease.Component;
                comp.Leases.Remove(lease);
                SystemGroup group = lease.SystemGroup;
                group.Leases.Remove(lease);

                Department dept = lease.Department;
                dept.Leases.Remove(lease);

                db.Entry(lease).State = EntityState.Deleted;
                db.SaveChanges();
			}

            return new RedirectResult(Url.Action("DataRequested", "Billing"));
		}

		public void SetUpGrid(Trirand.Web.Mvc.JQGrid ordersGrid)
		{
			// Customize/change some of the default settings for this model
			// ID is a mandatory field. Must by unique if you have several grids on one page.
			ordersGrid.ID = "BillingGrid";
			// Setting the DataUrl to an action (method) in the controller is required.
			// This action will return the data needed by the grid
			ordersGrid.DataUrl = Url.Action("DataRequested");
			ordersGrid.EditUrl = Url.Action("EditRows");
		}

        private string ValidationEditRow(JQGridEditData e)
        {
            return "";
        }
    }
}
