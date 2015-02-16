using aulease.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace LeasingDatabase.Billing
{
    public class BillingReport
    {
        public static void SimpleReport(AuleaseEntities db, DateTime statementDate, out MemoryStream OutputStream)
        {
            DateTime sanitizedStatementDate = new DateTime(statementDate.Year, statementDate.Month, 15);

            IEnumerable<Lease> leases = db.Leases.Where(n => n.BeginDate <= sanitizedStatementDate && n.EndDate >= sanitizedStatementDate && n.MonthlyCharge != null);
            IEnumerable<Department> depts = leases.Select(n => n.Department).OrderBy(n => n.Fund).ThenBy(n => n.Org).ThenBy(n => n.Program).Distinct().ToList();
            OutputStream = new MemoryStream();
            TextWriter tw = new StreamWriter(OutputStream);

            foreach (var dept in depts)
            {
                tw.WriteLine(dept.Fund + "-" + dept.Org + "-" + dept.Program + ", " + dept.Leases.Where(n => n.BeginDate <= statementDate && n.EndDate >= statementDate && n.MonthlyCharge != null).Sum(n => n.MonthlyCharge).ToString());
            }

            tw.Flush();
        }

        public static void DetailedReport(AuleaseEntities db, DateTime statementDate, Department dept, out MemoryStream OutputStream)
        {
            OutputStream = new MemoryStream();

            ExcelPackage p = new ExcelPackage();

            p.Workbook.Properties.Author = "Ben Fogel";
            p.Workbook.Properties.Title = "Billing Report";
            p.Workbook.Properties.Company = "AU Lease";

            p.Workbook.Worksheets.Add("New Leases");
            ExcelWorksheet ws1 = p.Workbook.Worksheets[1];
            ws1.Name = "New Leases";

            CreateHeadingsForWorksheet(ws1);

            List<Component> newComps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().MonthlyCharge != null && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().DepartmentId == dept.Id && n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().BeginDate.Value.Month == statementDate.Month && n.Leases.OrderBy(o => o.EndDate).FirstOrDefault().BeginDate.Value.Year == statementDate.Year && n.Leases.Count == 1).ToList();

            for (int i = 0; i < newComps.Count; i++)
            {
                Component comp = newComps[i];
                ws1.Cells[i + 2, 1].Value = comp.SerialNumber;
                ws1.Cells[i + 2, 2].Value = comp.LeaseTag;
                ws1.Cells[i + 2, 3].Value = comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.FirstName + " " + comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.LastName;
                ws1.Cells[i + 2, 4].Value = comp.TypeId.HasValue ? comp.Type.Name : "";
                ws1.Cells[i + 2, 5].Value = comp.ModelId.HasValue ? comp.Model.Name : "";
                ws1.Cells[i + 2, 6].Value = comp.Leases.FirstOrDefault().BeginDate;
                ws1.Cells[i + 2, 7].Value = comp.Leases.FirstOrDefault().EndDate;
                ws1.Cells[i + 2, 8].Value = comp.Leases.FirstOrDefault().MonthlyCharge;
                ws1.Cells[i + 2, 9].Value = comp.Leases.FirstOrDefault().OverheadId.HasValue ? comp.Leases.FirstOrDefault().Overhead.Term : 0;
                ws1.Cells[i + 2, 10].Value = comp.Leases.FirstOrDefault().Department.Fund + comp.Leases.FirstOrDefault().Department.Org + comp.Leases.FirstOrDefault().Department.Program;
            }

            AutofitColumns(ws1);

            p.Workbook.Worksheets.Add("Expiring Leases");
            ExcelWorksheet ws2 = p.Workbook.Worksheets[2];
            ws2.Name = "Expiring Leases";

            CreateHeadingsForWorksheet(ws2);

            DateTime expDate = statementDate.AddMonths(-1);
            List<Component> expComps = db.Components.Where(n => n.Leases.Where(o => o.DepartmentId == dept.Id).Count() > 0 && n.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(o => o.EndDate).FirstOrDefault().DepartmentId == dept.Id && n.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value.Month == expDate.Month && n.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value.Year == expDate.Year).ToList();

            for (int i = 0; i < expComps.Count; i++)
            {
                Component comp = expComps[i];
                ws2.Cells[i + 2, 1].Value = comp.SerialNumber;
                ws2.Cells[i + 2, 2].Value = comp.LeaseTag;
                ws2.Cells[i + 2, 3].Value = comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.FirstName + " " + comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.LastName;
                ws2.Cells[i + 2, 4].Value = comp.TypeId.HasValue ? comp.Type.Name : "";
                ws2.Cells[i + 2, 5].Value = comp.ModelId.HasValue ? comp.Model.Name : "";
                ws2.Cells[i + 2, 6].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().BeginDate;
                ws2.Cells[i + 2, 7].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().EndDate;
                ws2.Cells[i + 2, 8].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().MonthlyCharge;
                ws2.Cells[i + 2, 9].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().Overhead.Term;
                ws2.Cells[i + 2, 10].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Fund + comp.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Org + comp.Leases.Where(o => o.DepartmentId == dept.Id).OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Program;
            }

            AutofitColumns(ws2);

            p.Workbook.Worksheets.Add("Current Leases");
            ExcelWorksheet ws3 = p.Workbook.Worksheets[3];
            ws3.Name = "Current Leases";

            CreateHeadingsForWorksheet(ws3);

            List<Component> comps = db.Leases.Where(o => o.DepartmentId == dept.Id && o.BeginDate <= statementDate && o.EndDate >= statementDate && o.MonthlyCharge != null).Select(n => n.Component).Distinct().ToList();

            for (int i = 0; i < comps.Count; i++)
            {
                Component comp = comps[i];
                ws3.Cells[i + 2, 1].Value = comp.SerialNumber;
                ws3.Cells[i + 2, 2].Value = comp.LeaseTag;
                ws3.Cells[i + 2, 3].Value = comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.FirstName + " " + comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.LastName;
                ws3.Cells[i + 2, 4].Value = comp.TypeId.HasValue ? comp.Type.Name : "";
                ws3.Cells[i + 2, 5].Value = comp.ModelId.HasValue ? comp.Model.Name : "";
                ws3.Cells[i + 2, 6].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().BeginDate;
                ws3.Cells[i + 2, 7].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().EndDate;
                ws3.Cells[i + 2, 8].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().MonthlyCharge;
                ws3.Cells[i + 2, 9].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().Overhead.Term;
                ws3.Cells[i + 2, 10].Value = comp.Leases.Where(o => o.DepartmentId == dept.Id && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().Department.Fund + comp.Leases.Where(o => o.DepartmentId == dept.Id && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().Department.Org + comp.Leases.Where(o => o.DepartmentId == dept.Id && o.BeginDate <= statementDate && o.EndDate >= statementDate).FirstOrDefault().Department.Program;
            }

            AutofitColumns(ws3);

            // END OF WRITING EXCEL FILE //

            p.SaveAs(OutputStream);
        }

        private static void AutofitColumns(ExcelWorksheet ws)
        {
            for (int i = 1; i <= 10; i++)
            {
                ws.Column(i).AutoFit();
            }
        }

        private static void CreateHeadingsForWorksheet(ExcelWorksheet ws)
        {
            for (int colIndex = 1; colIndex <= 12; colIndex ++ )
            {
                var fill = ws.Cells[1, colIndex].Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.LightGray);
            }

            ws.Cells[1, 1].Value = "Serial_Number";
            ws.Cells[1, 2].Value = "AU_Lease_Tag";
            ws.Cells[1, 3].Value = "Name";
            ws.Cells[1, 4].Value = "Component_Type";
            ws.Cells[1, 5].Value = "Model";
            ws.Cells[1, 6].Value = "BeginBillingDate";
            ws.Cells[1, 7].Value = "EndBillingDate";
            ws.Cells[1, 8].Value = "Reg_Mo_Chg";
            ws.Cells[1, 9].Value = "Term";
            ws.Cells[1, 10].Value = "FOP";

            ws.Column(6).Style.Numberformat.Format = "mm-dd-yyyy";
            ws.Column(7).Style.Numberformat.Format = "mm-dd-yyyy";
            ws.Column(8).Style.Numberformat.Format = "#,##0.00";
            ws.Column(9).Style.Numberformat.Format = "##";
            ws.Column(10).Style.Numberformat.Format = "@";
        }


    }
}
