using aulease.Entities;
using CWSToolkit;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/
        private static MetaDbContext mdb = new MetaDbContext();

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult GenerateBilling()
		{
			ViewBag.dates = FillDropDownListDates();
			return View();
		}

		[HttpPost]
		public ActionResult GenerateBilling(string Date)
		{
			DateTime statementDate = Convert.ToDateTime(Date);
            statementDate = new DateTime(statementDate.Year, statementDate.Month, 15);

			AuleaseEntities db = new AuleaseEntities();

			IEnumerable<Lease> leases = db.Leases.Where(n => n.BeginDate <= statementDate && n.EndDate >= statementDate && n.MonthlyCharge != null);
			IEnumerable<Department> tempdepts = leases.Select(n => n.Department).OrderBy(n => n.Fund).ThenBy(n => n.Org).ThenBy(n => n.Program);
			IEnumerable<Department> depts = tempdepts.Distinct();
			MemoryStream OutputStream = new MemoryStream();
			TextWriter tw = new StreamWriter(OutputStream);

			foreach (var dept in depts)
			{
				tw.WriteLine(dept.Fund + "-" + dept.Org + "-" + dept.Program + ", " + dept.Leases.Where(n => n.BeginDate <= statementDate && n.EndDate >= statementDate && n.MonthlyCharge != null).Sum(n => n.MonthlyCharge).ToString());
			}

			tw.Close();

			Response.Clear();
			Response.ContentType = "application/octate-stream";
			Response.AddHeader("content-disposition", "attachment;filename=" + statementDate.ToString("MMMyy") + ".txt");
			Response.AddHeader("Content-length", OutputStream.GetBuffer().Length.ToString());
			Response.OutputStream.Write(OutputStream.GetBuffer(), 0, OutputStream.GetBuffer().Length);
			Response.End();

			return RedirectToAction("GenerateBilling");
		}

		public ActionResult GenerateOITBilling()
		{
			ViewBag.dates = FillDropDownListDates();
			return View();
		}

		[HttpPost]
		public ActionResult GenerateOITBilling(string Date, string FOP)
		{
			DateTime statementDate = Convert.ToDateTime(Date);
			string Fund = FOP.Substring(0, 6);
			string Org = FOP.Substring(7, 6);
			string Program = FOP.Substring(14, 4);

			AuleaseEntities db = new AuleaseEntities();

            Department dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();

			MemoryStream OutputStream = new MemoryStream();

			ExcelPackage p = new ExcelPackage();

			p.Workbook.Properties.Author = "Ben Fogel";
			p.Workbook.Properties.Title = "Billing Report";
			p.Workbook.Properties.Company = "AU Lease";

			p.Workbook.Worksheets.Add("New Leases");
			ExcelWorksheet ws1 = p.Workbook.Worksheets[1];
			ws1.Name = "New Leases";

			int rowIndex = 1;
			int colIndex = 1;

			while (colIndex <= 12)
			{
				var cell = ws1.Cells[rowIndex, colIndex];
				var fill = cell.Style.Fill;
				fill.PatternType = ExcelFillStyle.Solid;
				fill.BackgroundColor.SetColor(Color.LightGray);
				colIndex++;
			}

			ws1.Cells[1, 1].Value = "Serial_Number";
			ws1.Cells[1, 2].Value = "AU_Lease_Tag";
			ws1.Cells[1, 3].Value = "Name";
			ws1.Cells[1, 4].Value = "Component_Type";
			ws1.Cells[1, 5].Value = "Model";
			ws1.Cells[1, 6].Value = "BeginBillingDate";
			ws1.Cells[1, 7].Value = "EndBillingDate";
			ws1.Cells[1, 8].Value = "Reg_Mo_Chg";
			ws1.Cells[1, 9].Value = "Term";
			ws1.Cells[1, 10].Value = "FOP";

			ws1.Column(6).Style.Numberformat.Format = "mm-dd-yyyy";
			ws1.Column(7).Style.Numberformat.Format = "mm-dd-yyyy";

			List<Component> newComps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().MonthlyCharge != null && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Fund == Fund && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Org == Org && n.Leases.OrderBy(o => o.EndDate).FirstOrDefault().Department.Program == Program && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().BeginDate.Value.Month == statementDate.Month && n.Leases.OrderBy(o => o.EndDate).FirstOrDefault().BeginDate.Value.Year == statementDate.Year && n.Leases.Count == 1).ToList();

			DataTable dt = new DataTable();
			dt.Columns.Add("Serial Number", typeof(string));
			dt.Columns.Add("Lease Tag", typeof(string));
			dt.Columns.Add("Full Name", typeof(string));
			dt.Columns.Add("Component", typeof(string));
			dt.Columns.Add("Model", typeof(string));
			dt.Columns.Add("Begin Bill Date", typeof(string));
			dt.Columns.Add("End Bill Date", typeof(string));
			dt.Columns.Add("Monthly Charge", typeof(decimal));
			dt.Columns.Add("Term", typeof(string));
			dt.Columns.Add("FOP", typeof(string));

			int c = 0;
			while (c < newComps.Count)
			{
				dt.Rows.Add(newComps[c].SerialNumber,
							newComps[c].LeaseTag,
                            newComps[c].Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.FirstName + " " + newComps[c].Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.LastName,
							newComps[c].TypeId.HasValue ? newComps[c].Type.Name: "",
							newComps[c].ModelId.HasValue ? newComps[c].Model.Name : "",
							newComps[c].Leases.FirstOrDefault().BeginDate,
							newComps[c].Leases.FirstOrDefault().EndDate,
							newComps[c].Leases.FirstOrDefault().MonthlyCharge,
							newComps[c].Leases.FirstOrDefault().OverheadId.HasValue ? newComps[c].Leases.FirstOrDefault().Overhead.Term : 0,
							newComps[c].Leases.FirstOrDefault().Department.Fund + newComps[c].Leases.FirstOrDefault().Department.Org + newComps[c].Leases.FirstOrDefault().Department.Program);
				c++;
			}

			int rowCount = dt.Rows.Count;

			for (int i = 0; i < rowCount; i++)
			{
				ws1.Cells[i + 2, 1].Value = dt.Rows[i][0];
				ws1.Cells[i + 2, 2].Value = dt.Rows[i][1];
				ws1.Cells[i + 2, 3].Value = dt.Rows[i][2];
				ws1.Cells[i + 2, 4].Value = dt.Rows[i][3];
				ws1.Cells[i + 2, 5].Value = dt.Rows[i][4];
				ws1.Cells[i + 2, 6].Value = dt.Rows[i][5];
				ws1.Cells[i + 2, 7].Value = dt.Rows[i][6];
				ws1.Cells[i + 2, 8].Value = dt.Rows[i][7];
				ws1.Cells[i + 2, 9].Value = dt.Rows[i][8];
				ws1.Cells[i + 2, 10].Value = dt.Rows[i][9];

			}

			ws1.Column(1).AutoFit();
			ws1.Column(2).AutoFit();
			ws1.Column(3).AutoFit();
			ws1.Column(4).AutoFit();
			ws1.Column(5).AutoFit();
			ws1.Column(6).AutoFit();
			ws1.Column(7).AutoFit();
			ws1.Column(8).AutoFit();
			ws1.Column(9).AutoFit();
			ws1.Column(10).AutoFit();

			//   EXPIRING LEASE WORKSHEET  //

			p.Workbook.Worksheets.Add("Expiring Leases");
			ExcelWorksheet ws2 = p.Workbook.Worksheets[2];
			ws2.Name = "Expiring Leases";

			rowIndex = 1;
			colIndex = 1;

			while (colIndex <= 12)
			{
				var cell = ws2.Cells[rowIndex, colIndex];
				var fill = cell.Style.Fill;
				fill.PatternType = ExcelFillStyle.Solid;
				fill.BackgroundColor.SetColor(Color.LightGray);
				colIndex++;
			}

			ws2.Cells[1, 1].Value = "Serial_Number";
			ws2.Cells[1, 2].Value = "AU_Lease_Tag";
			ws2.Cells[1, 3].Value = "Name";
			ws2.Cells[1, 4].Value = "Component_Type";
			ws2.Cells[1, 5].Value = "Model";
			ws2.Cells[1, 6].Value = "BeginBillingDate";
			ws2.Cells[1, 7].Value = "EndBillingDate";
			ws2.Cells[1, 8].Value = "Reg_Mo_Chg";
			ws2.Cells[1, 9].Value = "Term";
			ws2.Cells[1, 10].Value = "FOP";

			ws2.Column(6).Style.Numberformat.Format = "mm-dd-yyyy";
			ws2.Column(7).Style.Numberformat.Format = "mm-dd-yyyy";

			DateTime expDate = statementDate.AddMonths(-1);
            //List<Component> expComps = db.Components.Where(n => n.Leases.Where(o => o.Department.Fund == Fund).OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Fund == Fund && n.Leases.Where(o => o.Department.Org == Org).OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Org == Org && n.Leases.Where(o => o.Department.Program == Program).OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Program == Program && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value.Month == expDate.Month && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value.Year == expDate.Year).ToList();
            List<Component> expComps = db.Components.Where(n =>n.Leases.Where(o => o.DepartmentId == dept.Id).Count() > 0 && n.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(o => o.EndDate).FirstOrDefault().DepartmentId == dept.Id && n.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value.Month == expDate.Month && n.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value.Year == expDate.Year).ToList();

			DataTable dt2 = new DataTable();
			dt2.Columns.Add("Serial Number", typeof(string));
			dt2.Columns.Add("Lease Tag", typeof(string));
			dt2.Columns.Add("Full Name", typeof(string));
			dt2.Columns.Add("Component", typeof(string));
			dt2.Columns.Add("Model", typeof(string));
			dt2.Columns.Add("Begin Bill Date", typeof(string));
			dt2.Columns.Add("End Bill Date", typeof(string));
			dt2.Columns.Add("Monthly Charge", typeof(decimal));
			dt2.Columns.Add("Term", typeof(string));
			dt2.Columns.Add("FOP", typeof(string));

			int d = 0;
			while (d < expComps.Count)
			{
				dt2.Rows.Add(expComps[d].SerialNumber,
							expComps[d].LeaseTag,
                            expComps[d].Leases.FirstOrDefault().SystemGroup.User.FirstName + " " + expComps[d].Leases.FirstOrDefault().SystemGroup.User.LastName,
							expComps[d].Type.Name,
                            expComps[d].ModelId.HasValue ? expComps[d].Model.Name : "",
                            expComps[d].Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().BeginDate,
                            expComps[d].Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().EndDate,
                            expComps[d].Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().MonthlyCharge,
                            expComps[d].Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().Overhead.Term,
                            expComps[d].Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Fund + expComps[d].Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Org + expComps[d].Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Program);
				d++;
			}

			int rowCount2 = dt2.Rows.Count;

			for (int i = 0; i < rowCount2; i++)
			{
				ws2.Cells[i + 2, 1].Value = dt2.Rows[i][0];
				ws2.Cells[i + 2, 2].Value = dt2.Rows[i][1];
				ws2.Cells[i + 2, 3].Value = dt2.Rows[i][2];
				ws2.Cells[i + 2, 4].Value = dt2.Rows[i][3];
				ws2.Cells[i + 2, 5].Value = dt2.Rows[i][4];
				ws2.Cells[i + 2, 6].Value = dt2.Rows[i][5];
				ws2.Cells[i + 2, 7].Value = dt2.Rows[i][6];
				ws2.Cells[i + 2, 8].Value = dt2.Rows[i][7];
				ws2.Cells[i + 2, 9].Value = dt2.Rows[i][8];
				ws2.Cells[i + 2, 10].Value = dt2.Rows[i][9];

			}

			ws2.Column(1).AutoFit();
			ws2.Column(2).AutoFit();
			ws2.Column(3).AutoFit();
			ws2.Column(4).AutoFit();
			ws2.Column(5).AutoFit();
			ws2.Column(6).AutoFit();
			ws2.Column(7).AutoFit();
			ws2.Column(8).AutoFit();
			ws2.Column(9).AutoFit();
			ws2.Column(10).AutoFit();

			// CURRENT LEASE WORKSHEET //

			p.Workbook.Worksheets.Add("Current Leases");
			ExcelWorksheet ws3 = p.Workbook.Worksheets[3];
			ws3.Name = "Current Leases";

			rowIndex = 1;
			colIndex = 1;

			while (colIndex <= 12)
			{
				var cell = ws3.Cells[rowIndex, colIndex];
				var fill = cell.Style.Fill;
				fill.PatternType = ExcelFillStyle.Solid;
				fill.BackgroundColor.SetColor(Color.LightGray);
				colIndex++;
			}

			ws3.Cells[1, 1].Value = "Serial_Number";
			ws3.Cells[1, 2].Value = "AU_Lease_Tag";
			ws3.Cells[1, 3].Value = "Name";
			ws3.Cells[1, 4].Value = "Component_Type";
			ws3.Cells[1, 5].Value = "Model";
			ws3.Cells[1, 6].Value = "BeginBillingDate";
			ws3.Cells[1, 7].Value = "EndBillingDate";
			ws3.Cells[1, 8].Value = "Reg_Mo_Chg";
			ws3.Cells[1, 9].Value = "Term";
			ws3.Cells[1, 10].Value = "FOP";

			ws3.Column(6).Style.Numberformat.Format = "mm-dd-yyyy";
			ws3.Column(7).Style.Numberformat.Format = "mm-dd-yyyy";


            List<Lease> leases = db.Leases.Where(o => o.Department.Fund == Fund && o.Department.Org == Org && o.Department.Program == Program && o.BeginDate <= statementDate && o.EndDate >= statementDate && o.MonthlyCharge != null).ToList();
            List<Component> comps = leases.Select(n => n.Component).Distinct().ToList();// n.Leases.OrderBy(o => o.EndDate).FirstOrDefault().Department.Fund == Fund && n.Leases.OrderBy(o => o.EndDate).FirstOrDefault().Department.Org == Org && n.Leases.OrderBy(o => o.EndDate).FirstOrDefault().Department.Program == Program && n.Leases.OrderBy(o => o.EndDate).FirstOrDefault().BeginDate <= statementDate && n.Leases.OrderBy(o => o.EndDate).FirstOrDefault().EndDate >= statementDate).ToList();

			DataTable dt3 = new DataTable();
			dt3.Columns.Add("Serial Number", typeof(string));
			dt3.Columns.Add("Lease Tag", typeof(string));
			dt3.Columns.Add("Full Name", typeof(string));
			dt3.Columns.Add("Component", typeof(string));
			dt3.Columns.Add("Model", typeof(string));
			dt3.Columns.Add("Begin Bill Date", typeof(string));
			dt3.Columns.Add("End Bill Date", typeof(string));
			dt3.Columns.Add("Monthly Charge", typeof(decimal));
			dt3.Columns.Add("Term", typeof(string));
			dt3.Columns.Add("FOP", typeof(string));

			int e = 0;
			while (e < comps.Count)
			{
				dt3.Rows.Add(comps[e].SerialNumber,
							comps[e].LeaseTag,
							comps[e].Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.FirstName + " " + comps[e].Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.LastName,
							comps[e].Type != null ? comps[e].Type.Name : "",
							comps[e].ModelId.HasValue ? comps[e].Model.Name : "",
                            comps[e].Leases.Where(o => o.Department.Fund == Fund && o.Department.Org == Org && o.Department.Program == Program && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().BeginDate,
                            comps[e].Leases.Where(o => o.Department.Fund == Fund && o.Department.Org == Org && o.Department.Program == Program && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().EndDate,
                            comps[e].Leases.Where(o => o.Department.Fund == Fund && o.Department.Org == Org && o.Department.Program == Program && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().MonthlyCharge,
                            comps[e].Leases.Where(o => o.Department.Fund == Fund && o.Department.Org == Org && o.Department.Program == Program && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().Overhead.Term,
                            comps[e].Leases.Where(o => o.Department.Fund == Fund && o.Department.Org == Org && o.Department.Program == Program && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().Department.Fund + comps[e].Leases.Where(o => o.Department.Fund == Fund && o.Department.Org == Org && o.Department.Program == Program && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().Department.Org + comps[e].Leases.Where(o => o.Department.Fund == Fund && o.Department.Org == Org && o.Department.Program == Program && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().Department.Program);
				e++;
			}

			int rowCount3 = dt3.Rows.Count;

			for (int i = 0; i < rowCount3; i++)
			{
				ws3.Cells[i + 2, 1].Value = dt3.Rows[i][0];
				ws3.Cells[i + 2, 2].Value = dt3.Rows[i][1];
				ws3.Cells[i + 2, 3].Value = dt3.Rows[i][2];
				ws3.Cells[i + 2, 4].Value = dt3.Rows[i][3];
				ws3.Cells[i + 2, 5].Value = dt3.Rows[i][4];
				ws3.Cells[i + 2, 6].Value = dt3.Rows[i][5];
				ws3.Cells[i + 2, 7].Value = dt3.Rows[i][6];
				ws3.Cells[i + 2, 8].Value = dt3.Rows[i][7];
				ws3.Cells[i + 2, 9].Value = dt3.Rows[i][8];
				ws3.Cells[i + 2, 10].Value = dt3.Rows[i][9];

			}

			ws3.Column(1).AutoFit();
			ws3.Column(2).AutoFit();
			ws3.Column(3).AutoFit();
			ws3.Column(4).AutoFit();
			ws3.Column(5).AutoFit();
			ws3.Column(6).AutoFit();
			ws3.Column(7).AutoFit();
			ws3.Column(8).AutoFit();
			ws3.Column(9).AutoFit();
			ws3.Column(10).AutoFit();

			// END OF WRITING EXCEL FILE //

			p.SaveAs(OutputStream);

            //Response.Clear();
            //Response.ContentType = "application/octet-stream";
            //Response.AddHeader("content-disposition", "attachment;filename=AdminBilling-" + statementDate.ToString("MMMyyyy") + ".xlsx");
            //Response.AddHeader("Content-length", OutputStream.ToArray().Length.ToString());
            //Response.OutputStream.Write(OutputStream.GetBuffer(), 0, OutputStream.GetBuffer().Length);
            //Response.End();

            OutputStream.Position = 0;

            return File(OutputStream, "application/xlsx", "AdminBilling-" + statementDate.ToString("MMMyyyy") + ".xlsx");

			//return RedirectToAction("GenerateOITBilling");
		}

		public List<SelectListItem> FillDropDownListDates()
		{
			List<string> list = new List<string>();
			DateTime past = Convert.ToDateTime("12/1/2005");

			while ((DateTime.Now - past).Days > -28)
			{
				list.Add(past.Month + "/" + past.Year);
				past = past.AddMonths(1);
			}
			list.Reverse();
			List<SelectListItem> dates = list.Select(n => new SelectListItem { Value = n, Text = n }).ToList();
			return dates;
		}

        public ActionResult GenerateRateDevelopmentReport()
        {
            AuleaseEntities db = new AuleaseEntities();
            List<aulease.Entities.Type> types = db.Types.Where(n => n.Components.Count > 0).ToList();

            foreach (var type in types)
            {
                //List<Overhead> overheads = db.Overheads.Where(n => n.Type)
            }

            return View();
        }

    }
}
