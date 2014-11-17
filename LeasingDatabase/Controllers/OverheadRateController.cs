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
    public class OverheadRateController : Controller
    {
        //
        // GET: /OverheadRate/

        public ActionResult Index()
        {
            var gridModel = new LeasingDatabase.Models.Grid.OverheadRateJqGridModel();
            var ordersGrid = gridModel.OrdersGrid;

            SetUpGrid(ordersGrid);

            return View(gridModel);
        }

        public JsonResult DataRequested()
        {
            var gridModel = new LeasingDatabase.Models.Grid.OverheadRateJqGridModel();
            var db = new AuleaseEntities();

            SetUpGrid(gridModel.OrdersGrid);

            List<Overhead> overhead = new List<Overhead>();

            var Overhead = db.Overheads.Select(n => new { n.TypeId, n.Term, n.RateLevel }).Distinct();

            foreach (var over in Overhead)
            {
                overhead.Add(db.Overheads.Where(n => n.TypeId == over.TypeId && n.Term == over.Term && n.RateLevel == over.RateLevel).OrderByDescending(n => n.BeginDate).FirstOrDefault());
            }

            var model = overhead.Select(n => new OverheadModel
            {
                Id = n.Id,
                Type = n.Type.Name,
                Rate = n.Rate,
                RateLevel = n.RateLevel,
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
                string Type = e.RowData["Type"].ToString();
                string RateLevel = e.RowData["RateLevel"].ToString();
                decimal Rate = Convert.ToDecimal(e.RowData["Rate"]);

                Overhead overhead = new Overhead();
                overhead.Term = Term;
                overhead.Type = db.Types.Where(n => n.Name == Type).Single();
                overhead.RateLevel = RateLevel;
                overhead.Rate = Rate;
                overhead.BeginDate = DateTime.Now;

                db.Overheads.Add(overhead);

                db.SaveChanges();
            }
            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
            {
                // Not implemented
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
            ordersGrid.ID = "OverheadRateGrid";
            // Setting the DataUrl to an action (method) in the controller is required.
            // This action will return the data needed by the grid
            ordersGrid.DataUrl = Url.Action("DataRequested");
            ordersGrid.EditUrl = Url.Action("EditRows");
        }

    }
}
