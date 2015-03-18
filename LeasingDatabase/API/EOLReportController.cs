using aulease.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class EOLReportController : ApiController
    {
        // GET api/eolreport/5
        public HttpResponseMessage Get(DateTime date)
        {
            AuleaseEntities db = new AuleaseEntities();

            var OutputStream = new MemoryStream();

            ExcelPackage p = new ExcelPackage();

            p.Workbook.Properties.Author = "Ben Fogel";
            p.Workbook.Properties.Title = "EOL Report";
            p.Workbook.Properties.Company = "AU Lease";

            p.Workbook.Worksheets.Add("EOL");
            ExcelWorksheet ws1 = p.Workbook.Worksheets[1];
            ws1.Name = "EOL";

            CreateHeadingsForWorksheet(ws1);

            IEnumerable<Component> Components = db.Components.Where(n => n.ReturnDate.HasValue && n.ReturnDate.Value.Month == date.Month && n.ReturnDate.Value.Year == date.Year);

            List<EOLModel> models = Components.Select(n => new EOLModel
            {
                id = n.Id,
                ShortSerialNumber = n.SerialNumber.Length >= 7 ? n.SerialNumber.Substring(n.SerialNumber.Length - 7, 7) : n.SerialNumber,
                SerialNumber = n.SerialNumber,
                LeaseTag = n.LeaseTag,
                Type = n.Type.Name,
                Make = n.Make.Name,
                Model = n.Model.Name,
                StatementName = n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().StatementName,
                GID = n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.User.GID,
                EndBillingDate = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value,
                ReturnDate = n.ReturnDate.Value,
                Damages = n.Damages ?? "",
                Decommissioned = (n.Status.Name == "Ready to Ship"),
                Term = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Overhead.Term,
                MonthlyCharge = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().MonthlyCharge.Value,
                SR = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.PO.PONumber,
                DepartmentName = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Name,
                FOP = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.GetFOP(),
                RateLevel = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Overhead.RateLevel
            }).ToList();

            for (int i = 0; i < models.Count(); i++)
            {
                int RowIndex = i + 2;
                EOLModel model = models[i];
                ws1.Cells[RowIndex, 1].Value = model.ShortSerialNumber;
                ws1.Cells[RowIndex, 2].Value = model.SR;
                ws1.Cells[RowIndex, 3].Value = model.StatementName;
                ws1.Cells[RowIndex, 4].Value = model.DepartmentName;
                ws1.Cells[RowIndex, 5].Value = model.Type;
                ws1.Cells[RowIndex, 6].Value = model.Make;
                ws1.Cells[RowIndex, 7].Value = model.Model;
                ws1.Cells[RowIndex, 8].Value = model.LeaseTag;
                ws1.Cells[RowIndex, 9].Value = model.SerialNumber;
                ws1.Cells[RowIndex, 10].Value = model.FOP;
                ws1.Cells[RowIndex, 11].Value = model.RateLevel;
                ws1.Cells[RowIndex, 12].Value = model.Term;
                ws1.Cells[RowIndex, 13].Value = model.MonthlyCharge;
                ws1.Cells[RowIndex, 14].Value = model.GID;
                ws1.Cells[RowIndex, 15].Value = model.EndBillingDate;
                ws1.Cells[RowIndex, 16].Value = model.ReturnDate;
                ws1.Cells[RowIndex, 17].Value = model.Damages;
            }

            AutofitColumns(ws1);
            p.SaveAs(OutputStream);

            OutputStream.Position = 0;
            string applicationType = "application/xlsx";
            string fileName = "EOL-" + date.ToString("MMMyyyy") + ".xlsx";

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(OutputStream.ToArray());
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(applicationType);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = fileName };

            return result;
        }

        private static void AutofitColumns(ExcelWorksheet ws)
        {
            for (int i = 1; i <= 17; i++)
            {
                ws.Column(i).AutoFit();
            }
        }

        private static void CreateHeadingsForWorksheet(ExcelWorksheet ws)
        {
            for (int colIndex = 1; colIndex <= 12; colIndex++)
            {
                var fill = ws.Cells[1, colIndex].Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.LightGray);
            }

            //ws.Cells[1, 1].Value = "SN";
            //ws.Cells[1, 2].Value = "LeaseTag";
            //ws.Cells[1, 3].Value = "Term";
            //ws.Cells[1, 4].Value = "Monthly Charge";
            //ws.Cells[1, 5].Value = "SR";
            //ws.Cells[1, 6].Value = "Serial Number";
            //ws.Cells[1, 7].Value = "Type";
            //ws.Cells[1, 8].Value = "Make";
            //ws.Cells[1, 9].Value = "Model";
            //ws.Cells[1, 10].Value = "Department Name";
            //ws.Cells[1, 11].Value = "FOP";
            //ws.Cells[1, 12].Value = "Rate Level";
            //ws.Cells[1, 13].Value = "Statement Name";
            //ws.Cells[1, 14].Value = "GID";
            //ws.Cells[1, 15].Value = "End Billing Date";
            //ws.Cells[1, 16].Value = "Return Date";
            //ws.Cells[1, 17].Value = "Damages";

            ws.Cells[1, 1].Value = "SN";
            ws.Cells[1, 2].Value = "SR";
            ws.Cells[1, 3].Value = "Statement Name";
            ws.Cells[1, 4].Value = "Department Name";
            ws.Cells[1, 5].Value = "Type";
            ws.Cells[1, 6].Value = "Make";
            ws.Cells[1, 7].Value = "Model";
            ws.Cells[1, 8].Value = "Lease Tag";
            ws.Cells[1, 9].Value = "Serial Number";
            ws.Cells[1, 10].Value = "FOP";
            ws.Cells[1, 11].Value = "Rate Level";
            ws.Cells[1, 12].Value = "Term";
            ws.Cells[1, 13].Value = "Monthly Charge";
            ws.Cells[1, 14].Value = "GID";
            ws.Cells[1, 15].Value = "End Billing Date";
            ws.Cells[1, 16].Value = "Return Date";
            ws.Cells[1, 17].Value = "Damages";

            ws.Column(12).Style.Numberformat.Format = "##";
            ws.Column(13).Style.Numberformat.Format = "#,##0.00";
            ws.Column(7).Style.Numberformat.Format = "@";
            ws.Column(15).Style.Numberformat.Format = "mm-dd-yyyy";
            ws.Column(16).Style.Numberformat.Format = "mm-dd-yyyy";
        }

    }
}
