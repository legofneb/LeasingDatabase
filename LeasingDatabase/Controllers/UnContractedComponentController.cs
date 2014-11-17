using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Core;
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
    public class UnContractedComponentController : Controller
    {
        //
        // GET: /UnContractedComponent/
        [AuthorizeUser("Admin")]
        public ActionResult Index()
        {
            var gridModel = new LeasingDatabase.Models.Grid.ComponentJqGridModel();
            var ordersGrid = gridModel.OrdersGrid;

            SetUpGrid(ordersGrid);

            return View(gridModel);
        }

        public JsonResult DataRequested()
        {
            var gridModel = new LeasingDatabase.Models.Grid.ComponentJqGridModel();
            var db = new AuleaseEntities();

            SetUpGrid(gridModel.OrdersGrid);

            var comps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().MonthlyCharge != null && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().ContractNumber == null);
            // Performance increase if taken from first lease perspective???

            var model = comps.OrderByDescending(n => n.SystemGroupId).Select(n => new ComponentModel
            {
                ComponentID = n.Id,
                SerialNumber = n.SerialNumber,
                LeaseTag = n.LeaseTag,
                Note = n.Note,
                Make = n.Make.Name,
                ComponentType = n.Type.Name,
                Model = n.Model.Name,
                BeginDate = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().BeginDate.Value,
                EndDate = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value,
                StatementName = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().StatementName,
                ContractNumber = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().ContractNumber,
                DepartmentName = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Name,
                Fund = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Fund,
                Org = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Org,
                Program = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Program,
                MonthlyCharge = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().MonthlyCharge.Value,
                RateLevel = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Overhead.RateLevel,
                Term = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Overhead.Term,
                SRNumber = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.PO.PONumber,
                GID = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.User.GID,
                Phone = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.User.Phone,
                Building = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.Location.Building,
                Room = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.Location.Room
            });

            return gridModel.OrdersGrid.DataBind(model);
        }

        public void EditRows()
        {
            var gridModel = new LeasingDatabase.Models.Grid.ComponentJqGridModel();
            var db = new AuleaseEntities();

            var e = gridModel.OrdersGrid.GetEditData(); // Edit Row

            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
            {
                int ComponentID = Convert.ToInt32(e.RowData["ComponentID"]);

                string _SerialNumber = e.RowData["SerialNumber"].ToString().Trim();
                string _LeaseTag = e.RowData["LeaseTag"].ToString().Trim();
                string _Note = e.RowData["Note"].ToString().Trim();
                string _Make = e.RowData["Make"].ToString().Trim();
                string _ComponentType = e.RowData["ComponentType"].ToString().Trim();
                string _Model = e.RowData["Model"].ToString().Trim();

                // Update Component

                Component comp = db.Components.Where(n => n.Id == ComponentID).Single();
                comp.SerialNumber = _SerialNumber;
                comp.LeaseTag = _LeaseTag;
                comp.Note = _Note;
                comp.Make = db.Makes.Where(n => n.Name == _Make).Single();
                comp.Type = db.Types.Where(n => n.Name == _ComponentType).Single();
                Model model;
                if (db.Models.Any(n => n.Name == _Model))
                {
                    model = db.Models.Where(n => n.Name == _Model).FirstOrDefault();
                }
                else
                {
                    model = new Model() { Name = _Model, Make = db.Makes.Where(n => n.Name == _Make).Single() };
                }
                comp.Model = model;


                // Update Leases

                DateTime _BeginDate = Convert.ToDateTime(e.RowData["BeginDate"]);
                DateTime _EndDate = Convert.ToDateTime(e.RowData["EndDate"]);
                string _StatementName = e.RowData["StatementName"].ToString().Trim();
                string _ContractNumber = e.RowData["ContractNumber"].ToString().Trim();
                string _Fund = e.RowData["Fund"].ToString().Trim();
                string _Org = e.RowData["Org"].ToString().Trim();
                string _Program = e.RowData["Program"].ToString().Trim();
                string _RateLevel = e.RowData["RateLevel"].ToString().Trim();
                int _Term = Convert.ToInt32(e.RowData["Term"]);
                string _GID = e.RowData["GID"].ToString().Trim();

                Lease currentLease = comp.Leases.OrderByDescending(n => n.Timestamp).First();

                Department department;
                if (db.Departments.Any(n => n.Fund == _Fund && n.Org == _Org && n.Program == _Program))
                {
                    department = db.Departments.Where(n => n.Fund == _Fund && n.Org == _Org && n.Program == _Program).FirstOrDefault();
                }
                else
                {
                    department = new Department { Fund = _Fund, Org = _Org, Program = _Program };
                }

                Overhead Overhead = db.Overheads.Where(n => n.RateLevel == _RateLevel && n.Term == _Term).OrderByDescending(n => n.BeginDate).FirstOrDefault();

                User newUser;

                if (db.Users.Any(n => n.GID == _GID))
                {
                    newUser = db.Users.Where(n => n.GID == _GID).Single();
                }
                else
                {
                    newUser = new User();
                    newUser.GID = _GID;
                    newUser.Location = new Location();
                }

                currentLease.BeginDate = _BeginDate;
                currentLease.EndDate = _EndDate;
                currentLease.StatementName = _StatementName;
                currentLease.Timestamp = DateTime.Now;
                currentLease.ContractNumber = _ContractNumber;
                currentLease.Department = department;
                currentLease.Overhead = Overhead;
                currentLease.SystemGroup.User = newUser;

                db.SaveChanges();
            }
            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
            {
                // No Adding through this table!
                throw new NotImplementedException();
            }
            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.DeleteRow)
            {
                int ComponentID = Convert.ToInt32(e.RowData["ComponentID"]);
                Component comp = db.Components.Where(n => n.Id == ComponentID).Single();
                List<Lease> leases = comp.Leases.ToList();
                List<Charge> Charges = leases.SelectMany(n => n.Charges).ToList();
                int SystemGroupID = leases.Select(n => n.SystemGroupId).Single();
                int OrderID = leases.Select(n => n.SystemGroup).SingleOrDefault().OrderId;
                int? POID = leases.Select(n => n.SystemGroup).SingleOrDefault().POId.HasValue ? (int?)leases.Select(n => n.SystemGroup).Single().POId.Value : null;



                foreach (var charge in Charges)
                {
                    db.Entry(charge).State = EntityState.Deleted;
                }

                foreach (var lease in leases)
                {
                    db.Entry(lease).State = EntityState.Deleted;
                }

                List<Property> props = comp.Properties.ToList();
                foreach (var prop in props)
                {
                    comp.Properties.Remove(prop);
                }

                db.Entry(comp).State = EntityState.Deleted;
                db.SaveChanges();

                if (db.SystemGroups.Where(n => n.Id == SystemGroupID).SingleOrDefault().Leases.Count == 0)
                {
                    db.Entry(db.SystemGroups.Where(n => n.Id == SystemGroupID).SingleOrDefault()).State = EntityState.Deleted;
                }

                if (db.Orders.Where(n => n.Id == OrderID).Single().SystemGroups.Count == 0)
                {
                    db.Entry(db.Orders.Where(n => n.Id == OrderID).Single()).State = EntityState.Deleted;
                }

                if (POID != null && db.POes.Where(n => n.Id == POID).Single().SystemGroups.Count == 0)
                {
                    db.Entry(db.POes.Where(n => n.Id == POID).Single()).State = EntityState.Deleted;
                }

                db.SaveChanges();
            }
        }

        public void SetUpGrid(Trirand.Web.Mvc.JQGrid ordersGrid)
        {
            // Customize/change some of the default settings for this model
            // ID is a mandatory field. Must by unique if you have several grids on one page.
            ordersGrid.ID = "ComponentGrid";
            // Setting the DataUrl to an action (method) in the controller is required.
            // This action will return the data needed by the grid
            ordersGrid.DataUrl = Url.Action("DataRequested");
            ordersGrid.EditUrl = Url.Action("EditRows");
        }

        public JsonResult Extend(string SerialNumber, string EndDate)
        {
            AuleaseEntities db = new AuleaseEntities();
            Component comp = db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
            Lease lastLease = comp.Leases.OrderByDescending(n => n.EndDate).First();
            DateTime d = lastLease.EndDate.Value;

            DateTime begDate = new DateTime(d.AddMonths(1).Year, d.AddMonths(1).Month, 1);
            DateTime endDate = UnixTimeStampToDateTime(Convert.ToDouble(EndDate));

            Lease newLease = new Lease();
            newLease.Charges = lastLease.Charges;
            newLease.Component = lastLease.Component;
            newLease.ComponentId = lastLease.ComponentId;
            newLease.ContractNumber = lastLease.ContractNumber;
            newLease.Department = lastLease.Department;
            newLease.DepartmentId = lastLease.DepartmentId;
            newLease.MonthlyCharge = lastLease.MonthlyCharge;
            newLease.Overhead = lastLease.Overhead;
            newLease.OverheadId = lastLease.OverheadId;
            newLease.StatementName = lastLease.StatementName;
            newLease.SystemGroup = lastLease.SystemGroup;
            newLease.SystemGroupId = lastLease.SystemGroupId;
            newLease.Timestamp = DateTime.Now;

            newLease.BeginDate = begDate;
            newLease.EndDate = endDate;

            comp.Leases.Add(newLease);

            db.SaveChanges();

            var output = new
            {
                success = true
            };

            return Json(output);
        }

        public JsonResult BuyOut(string SerialNumber, string Charge, string LastDate, string buyOutDate, bool changeFOP, string Fund, string Org, string Program)
        {
            AuleaseEntities db = new AuleaseEntities();
            Component comp = db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
            Lease lastLease = comp.Leases.OrderByDescending(n => n.EndDate).First();

            DateTime d = UnixTimeStampToDateTime(Convert.ToDouble(buyOutDate));

            lastLease.EndDate = UnixTimeStampToDateTime(Convert.ToDouble(LastDate));

            DateTime begDate = new DateTime(d.Year, d.Month, 1);
            DateTime EndDate = new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month));

            Lease newLease = new Lease();
            newLease.Charges = lastLease.Charges;
            newLease.Component = lastLease.Component;
            newLease.ComponentId = lastLease.ComponentId;
            newLease.ContractNumber = lastLease.ContractNumber;

            if (changeFOP)
            {
                Department dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
                newLease.Department = dept;
            }
            else
            {
                newLease.Department = lastLease.Department;
                newLease.DepartmentId = lastLease.DepartmentId;
            }

            newLease.MonthlyCharge = Convert.ToDecimal(Charge);
            newLease.Overhead = lastLease.Overhead;
            newLease.OverheadId = lastLease.OverheadId;
            newLease.StatementName = lastLease.StatementName;
            newLease.SystemGroup = lastLease.SystemGroup;
            newLease.SystemGroupId = lastLease.SystemGroupId;
            newLease.Timestamp = DateTime.Now;

            newLease.BeginDate = begDate;
            newLease.EndDate = EndDate;

            comp.Leases.Add(newLease);
            db.SaveChanges();

            var output = new { success = true };
            return Json(output);
        }

        public JsonResult ChangeFOP(string SerialNumber, string StatementName, string Fund, string Org, string Program, string changeDate)
        {
            DateTime ChangeDate = UnixTimeStampToDateTime(Convert.ToDouble(changeDate));
            AuleaseEntities db = new AuleaseEntities();
            Department newDept;

            if (db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
            {
                newDept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
            }
            else
            {
                newDept = new Department();
                newDept.Fund = Fund;
                newDept.Org = Org;
                newDept.Program = Program;

                db.Departments.Add(newDept);
                db.SaveChanges();
            }

            Component comp = db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
            Lease lastLease = comp.Leases.OrderByDescending(n => n.EndDate).First();

            DateTime begDate = new DateTime(ChangeDate.Year, ChangeDate.Month, 1);
            DateTime endDate = lastLease.EndDate.Value;

            lastLease.EndDate = new DateTime(ChangeDate.AddMonths(-1).Year, ChangeDate.AddMonths(-1).Month, DateTime.DaysInMonth(ChangeDate.AddMonths(-1).Year, ChangeDate.AddMonths(-1).Month));

            Lease newLease = new Lease();
            newLease.StatementName = StatementName;
            newLease.Charges = lastLease.Charges;
            newLease.Component = lastLease.Component;
            newLease.ComponentId = lastLease.ComponentId;
            newLease.ContractNumber = lastLease.ContractNumber;
            newLease.Department = newDept;
            newLease.MonthlyCharge = lastLease.MonthlyCharge;
            newLease.Overhead = lastLease.Overhead;
            newLease.OverheadId = lastLease.OverheadId;
            newLease.Timestamp = DateTime.Now;

            SystemGroup oldSystemGroup = lastLease.SystemGroup;
            SystemGroup newSystemGroup = new SystemGroup();

            newSystemGroup.Location = oldSystemGroup.Location;
            newSystemGroup.PO = oldSystemGroup.PO;
            newSystemGroup.Order = oldSystemGroup.Order;
            newSystemGroup.User = oldSystemGroup.User;

            newLease.SystemGroup = newSystemGroup;

            newLease.BeginDate = begDate;
            newLease.EndDate = endDate;

            comp.Leases.Add(newLease);

            db.SaveChanges();

            var output = new { success = true };
            return Json(output);
        }

        public ActionResult BillingPopUp(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();

            //Checking to make sure the component is in the new database or old database
            Component singleComp = db.Components.Where(n => n.Id == Id).Single();


            // The condition checks if any billing charges exist in the new database
            // If no charges exist then fetch info from old db.
            if (singleComp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == singleComp.Type).Count() < 1 || singleComp.Leases.OrderByDescending(n => n.EndDate).First().Charges.Where(n => n.Type == singleComp.Type).Single().Tax == null)
            {
                var oldGroup = singleComp.Leases.OrderByDescending(n => n.EndDate).First().SystemGroup;
                LeasingDatabaseMVC.OldAuleaseEntities olddb = new LeasingDatabaseMVC.OldAuleaseEntities();

                if (olddb.Billings.Where(n => n.SerialNumber == singleComp.SerialNumber).Count() == 0)
                {
                    return View();
                }

                LeasingDatabaseMVC.Billing oldSingleComp = olddb.Billings.Where(n => n.SerialNumber == singleComp.SerialNumber).OrderByDescending(n => n.EndBillDate).First();

                ViewBag.Component1 = singleComp.Type.Name;
                ViewBag.MonthlyCharge1 = oldSingleComp.MonthlyCharge;
                ViewBag.ComponentCost1 = oldSingleComp.ComponentCost;
                ViewBag.InsuranceCost1 = oldSingleComp.InsuranceCost;
                ViewBag.WarrantyCost1 = oldSingleComp.WarrantyCost;
                ViewBag.ShippingCost1 = 0M;
                ViewBag.LeasingRate1 = oldSingleComp.LeasingRate;
                ViewBag.ChargedRate1 = oldSingleComp.ChargedRate;
                ViewBag.InsuranceRate1 = oldSingleComp.Insurance;
                ViewBag.Tax1 = oldSingleComp.Tax;
                ViewBag.IGFRate1 = oldSingleComp.igfRate;

                return View();
            }

            var group = db.Components.Where(n => n.Id == Id).Select(n => n.Leases.FirstOrDefault().SystemGroup).Single();
            List<Component> comps = group.Leases.Select(n => n.Component).Distinct().ToList();

            Component comp = comps.Where(n => n.Type.Name != "Monitor").Single();
            List<Component> monitors = comps.Where(n => n.Type.Name == "Monitor").ToList();

            int count = comps.Count;

            if (comp != null)
            {
                ViewBag.Component1 = comp.Type.Name;
                ViewBag.MonthlyCharge1 = Billing.CalculateMonthlyCharge(comp.Leases.OrderByDescending(n => n.EndDate).First().Id);
                ViewBag.ComponentCost1 = Billing.GetComponentCost(comp.Id);
                ViewBag.InsuranceCost1 = Billing.GetInsuranceCost(comp.Id);
                ViewBag.WarrantyCost1 = Billing.GetWarrantyCost(comp.Id);
                ViewBag.ShippingCost1 = Billing.GetShippingCost(comp.Id);
                ViewBag.LeasingRate1 = Billing.CalculateLeasingRate(comp.Id);
                ViewBag.ChargedRate1 = Billing.CalculateChargedRate(comp.Id);
                ViewBag.InsuranceRate1 = Billing.CalculateSecondaryChargeRate(comp.Id, "Insurance");
                ViewBag.WarrantyRate1 = Billing.CalculateSecondaryChargeRate(comp.Id, "Warranty");
                ViewBag.ShippingRate1 = Billing.CalculateSecondaryChargeRate(comp.Id, "Shipping");
                ViewBag.Tax1 = Billing.CalculateTax(comp.Id);
                ViewBag.IGFRate1 = Billing.GetIGFRate(comp.Id);

                ViewBag.Term = comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().Overhead.Term;

                if (count > 1)
                {
                    Component mon1 = monitors.OrderBy(n => n.Id).First();

                    ViewBag.Component2 = mon1.Type.Name;
                    ViewBag.MonthlyCharge2 = Billing.CalculateMonthlyCharge(mon1.Leases.OrderByDescending(n => n.EndDate).First().Id);
                    ViewBag.ComponentCost2 = Billing.GetComponentCost(mon1.Id);
                    ViewBag.InsuranceCost2 = Billing.GetInsuranceCost(mon1.Id);
                    ViewBag.WarrantyCost2 = Billing.GetWarrantyCost(mon1.Id);
                    ViewBag.ShippingCost2 = Billing.GetShippingCost(mon1.Id);
                    ViewBag.LeasingRate2 = Billing.CalculateLeasingRate(mon1.Id);
                    ViewBag.ChargedRate2 = Billing.CalculateChargedRate(mon1.Id);
                    ViewBag.InsuranceRate2 = Billing.CalculateSecondaryChargeRate(mon1.Id, "Insurance");
                    ViewBag.WarrantyRate2 = Billing.CalculateSecondaryChargeRate(mon1.Id, "Warranty");
                    ViewBag.ShippingRate2 = Billing.CalculateSecondaryChargeRate(mon1.Id, "Shipping");
                    ViewBag.Tax2 = Billing.CalculateTax(mon1.Id);
                    ViewBag.IGFRate2 = Billing.GetIGFRate(mon1.Id);

                }

                if (count > 2)
                {
                    Component mon2 = monitors.OrderBy(n => n.Id).Skip(1).First();

                    ViewBag.Component3 = mon2.Type.Name;
                    ViewBag.MonthlyCharge3 = Billing.CalculateMonthlyCharge(mon2.Leases.OrderByDescending(n => n.EndDate).First().Id);
                    ViewBag.ComponentCost3 = Billing.GetComponentCost(mon2.Id);
                    ViewBag.InsuranceCost3 = Billing.GetInsuranceCost(mon2.Id);
                    ViewBag.WarrantyCost3 = Billing.GetWarrantyCost(mon2.Id);
                    ViewBag.ShippingCost3 = Billing.GetShippingCost(mon2.Id);
                    ViewBag.LeasingRate3 = Billing.CalculateLeasingRate(mon2.Id);
                    ViewBag.ChargedRate3 = Billing.CalculateChargedRate(mon2.Id);
                    ViewBag.InsuranceRate3 = Billing.CalculateSecondaryChargeRate(mon2.Id, "Insurance");
                    ViewBag.WarrantyRate3 = Billing.CalculateSecondaryChargeRate(mon2.Id, "Warranty");
                    ViewBag.ShippingRate3 = Billing.CalculateSecondaryChargeRate(mon2.Id, "Shipping");
                    ViewBag.Tax3 = Billing.CalculateTax(mon2.Id);
                    ViewBag.IGFRate3 = Billing.GetIGFRate(mon2.Id);
                }
            }
            else
            {
                if (count > 1)
                {
                    Component mon1 = monitors.OrderBy(n => n.Id).First();

                    ViewBag.Component1 = mon1.Type.Name;
                    ViewBag.MonthlyCharge1 = Billing.CalculateMonthlyCharge(mon1.Leases.OrderByDescending(n => n.EndDate).First().Id);
                    ViewBag.ComponentCost1 = Billing.GetComponentCost(mon1.Id);
                    ViewBag.InsuranceCost1 = Billing.GetInsuranceCost(mon1.Id);
                    ViewBag.WarrantyCost1 = Billing.GetWarrantyCost(mon1.Id);
                    ViewBag.ShippingCost1 = Billing.GetShippingCost(mon1.Id);
                    ViewBag.LeasingRate1 = Billing.CalculateLeasingRate(mon1.Id);
                    ViewBag.ChargedRate1 = Billing.CalculateChargedRate(mon1.Id);
                    ViewBag.Tax1 = Billing.CalculateTax(mon1.Id);
                    ViewBag.IGFRate1 = Billing.GetIGFRate(mon1.Id);

                }

                if (count > 2)
                {
                    Component mon2 = monitors.OrderBy(n => n.Id).Skip(1).First();

                    ViewBag.Component2 = mon2.Type.Name;
                    ViewBag.MonthlyCharge2 = Billing.CalculateMonthlyCharge(mon2.Leases.OrderByDescending(n => n.EndDate).First().Id);
                    ViewBag.ComponentCost2 = Billing.GetComponentCost(mon2.Id);
                    ViewBag.InsuranceCost2 = Billing.GetInsuranceCost(mon2.Id);
                    ViewBag.WarrantyCost2 = Billing.GetWarrantyCost(mon2.Id);
                    ViewBag.ShippingCost2 = Billing.GetShippingCost(mon2.Id);
                    ViewBag.LeasingRate2 = Billing.CalculateLeasingRate(mon2.Id);
                    ViewBag.ChargedRate2 = Billing.CalculateChargedRate(mon2.Id);
                    ViewBag.Tax2 = Billing.CalculateTax(mon2.Id);
                    ViewBag.IGFRate2 = Billing.GetIGFRate(mon2.Id);
                }
            }

            return View();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public ActionResult UnFinalizePage(string SerialNumber)
        {
            AuleaseEntities db = new AuleaseEntities();
            SystemGroup group = db.Components.Where(n => n.SerialNumber == SerialNumber).Single().Leases.OrderByDescending(n => n.EndDate).First().SystemGroup;
            PO po = group.PO;
            List<Lease> leases = po.SystemGroups.SelectMany(n => n.Leases).ToList();
            string message = "";
            foreach (var lease in leases)
            {
                lease.MonthlyCharge = null;
                lease.BeginDate = null;
                lease.EndDate = null;
                lease.ContractNumber = null;
                message += lease.Component.SerialNumber + " has been unfinalized from the database. <br /> ";
            }

            db.SaveChanges();

            ViewBag.message = message;
            return View();
        }

        public ActionResult UnFinalize()
        {
            return View();
        }

        public ActionResult ExportToExcel()
        {

            var gridModel = new LeasingDatabase.Models.Grid.ComponentJqGridModel();
            var ordersGrid = gridModel.OrdersGrid;
            AuleaseEntities db = new AuleaseEntities();

            SetUpGrid(gridModel.OrdersGrid);

            var comps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().MonthlyCharge != null && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().ContractNumber == null);
            // Performance increase if taken from first lease perspective???

            var model = comps.OrderByDescending(n => n.SystemGroupId).Select(n => new ComponentModel
            {
                ComponentID = n.Id,
                SerialNumber = n.SerialNumber,
                LeaseTag = n.LeaseTag,
                Note = n.Note,
                Make = n.Make.Name,
                ComponentType = n.Type.Name,
                Model = n.Model.Name,
                BeginDate = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().BeginDate.Value,
                EndDate = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value,
                StatementName = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().StatementName,
                ContractNumber = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().ContractNumber,
                DepartmentName = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Name,
                Fund = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Fund,
                Org = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Org,
                Program = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Program,
                MonthlyCharge = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().MonthlyCharge.Value,
                RateLevel = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Overhead.RateLevel,
                Term = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Overhead.Term,
                SRNumber = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.PO.PONumber,
                GID = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.User.GID,
                Phone = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.User.Phone,
                Building = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.Location.Building,
                Room = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.Location.Room
            });

            ordersGrid.ExportToExcel(model, "grid.xls");


            return View();
        }

    }
}
