using aulease.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LeasingDatabase.Controllers
{
    public class SingleChargeController : Controller
    {
        //
        // GET: /SingleCharge/

        public ActionResult Index()
        {
            AuleaseEntities db = new AuleaseEntities();
            List<SingleCharge> charges = db.SingleCharges.OrderByDescending(n => n.Date).ToList();
            ViewBag.Charges = charges;
            return View();
        }

        public ActionResult Create(string GID, string Amount, string Note, string Fund, string Org, string Program, string Date)
        {
            AuleaseEntities db = new AuleaseEntities();
            SingleCharge charge = new SingleCharge();
            charge.GID = GID;
            charge.Price = Convert.ToDecimal(Amount);
            charge.Note = Note;

            if (db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
            {
                charge.Department = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
            }
            else
            {
                Department dept = new Department();
                dept.Fund = Fund;
                dept.Org = Org;
                dept.Program = Program;
                dept.Name = "";
                db.Departments.Add(dept);
                db.SaveChanges();

                charge.Department = dept;
            }

            charge.Date = Convert.ToDateTime(Date);
            charge.HasPaid = false;

            db.SingleCharges.Add(charge);
            db.SaveChanges();

            return View();
        }

        public ActionResult Edit(string id, string GID, string Amount, string Note, string Fund, string Org, string Program, string Date)
        {
            AuleaseEntities db = new AuleaseEntities();
            int Id = Convert.ToInt32(id);
            SingleCharge charge = db.SingleCharges.Where(n => n.Id == Id).Single();
            charge.GID = GID;
            charge.Price = Convert.ToDecimal(Amount);
            charge.Note = Note;
            charge.Date = Convert.ToDateTime(Date);

            if (db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
            {
                charge.Department = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
            }
            else
            {
                Department dept = new Department();
                dept.Fund = Fund;
                dept.Org = Org;
                dept.Program = Program;
                dept.Name = "";
                db.Departments.Add(dept);
                db.SaveChanges();

                charge.Department = dept;
            }

            db.SaveChanges();

            return View();
        }

        public ActionResult Delete(string id)
        {
            AuleaseEntities db = new AuleaseEntities();
            int Id = Convert.ToInt32(id);

            SingleCharge charge = db.SingleCharges.Where(n => n.Id == Id).Single();
            db.SingleCharges.Remove(charge);
            db.SaveChanges();
            return View();
        }

        public ActionResult Excel()
        {
            AuleaseEntities db = new AuleaseEntities();
            List<SingleCharge> charges = db.SingleCharges.ToList();
            var table = new DataTable("Misc Charges");
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("FOP", typeof(string));
            table.Columns.Add("code", typeof(string));
            table.Columns.Add("First Name", typeof(string));
            table.Columns.Add("Last Name", typeof(string));
            table.Columns.Add("Department", typeof(string));
            table.Columns.Add("Date", typeof(string));

            foreach (var charge in charges)
            {
                table.Rows.Add(charge.Price, charge.Note, charge.Department.Fund + "-" + charge.Department.Org + "-" + charge.Department.Program, "CSMISC", charge.FirstName, charge.LastName, charge.Department.Name, charge.Date.ToString("d"));
            }

            MemoryStream ms = new MemoryStream();
            using (ExcelPackage pck = new ExcelPackage(ms))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Charges");
                ws.Cells["A1"].LoadFromDataTable(table, true);
                ws.Cells.AutoFitColumns();
                pck.Save();

                return File(ms.ToArray(), "application/excel", "MiscCharges.xlsx");
            }
        }

    }
}
