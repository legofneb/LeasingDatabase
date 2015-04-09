using aulease.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class ReconcileAgainstInvoiceController : ApiController
    {
        // GET api/reconcileagainstinvoice
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/reconcileagainstinvoice/5
        public string Get(int id)
        {
            return "value";
        }

        public class ErrorMessageRow
        {
            public string SerialNumber { get; set; }
            public string Type { get; set; }
            public string ErrorType { get; set; }
        }

        public HttpResponseMessage Post()
        {
            var httpRequest = HttpContext.Current.Request;

            var file = httpRequest.Files[0];

            List<string> SerialNumbersInInvoice = new List<string>();
            List<ErrorMessageRow> ErrorMessages = new List<ErrorMessageRow>();

            using (ExcelPackage p = new ExcelPackage())
            {
                using (Stream stream = file.InputStream)
                {
                    p.Load(stream);

                    AuleaseEntities db = new AuleaseEntities();

                    ExcelWorksheet ws = p.Workbook.Worksheets.First();
                    int endRow = ws.Dimension.End.Row;
                    int BillThruDateColumnIndex = 6;
                    int MonthlyCostColumnIndex = 27;
                    int SerialNumberColumnIndex = 29;
                    int TypeColumnIndex = 34;
                    int SubtotalColumnIndex = 13;

                    for (int i = 2; i <= endRow; i++)
                    {
                        DateTime CurrentBillDate;
                        DateTime.TryParse(ws.Cells[i, BillThruDateColumnIndex].Value.ToString(), out CurrentBillDate);
                        decimal MonthlyCost;
                        decimal.TryParse(ws.Cells[i, MonthlyCostColumnIndex].Value.ToString(), out MonthlyCost);
                        string SerialNumber = ws.Cells[i, SerialNumberColumnIndex].Value.ToString();
                        CurrentBillDate = CurrentBillDate.AddDays(-15);
                        SerialNumbersInInvoice.Add(SerialNumber);

                        if (ws.Cells[i, TypeColumnIndex].Value.ToString().ToUpper().Contains("VENDOR SOURCED MAINTENANCE"))
                        {
                            continue;
                        }

                        int subtotal;
                        int.TryParse(ws.Cells[i, 13].Value.ToString(), out subtotal);

                        if (subtotal < 0)
                        {
                            continue;
                        }

                        DateTime EndDateBoundary = CurrentBillDate.AddMonths(-2);

                        if (db.Components.Where(n => n.Leases.Where(o => o.BeginDate.HasValue && o.BeginDate <= CurrentBillDate && o.EndDate.HasValue && o.EndDate.Value >= EndDateBoundary).Count() > 0).Any(n => n.SerialNumber == SerialNumber))
                        {
                            if (db.Components.Where(n => n.SerialNumber == SerialNumber).Count() > 1)
                            {
                                ErrorMessages.Add(new ErrorMessageRow { SerialNumber = SerialNumber, ErrorType = "Duplicate Serial Numbers found in database", Type = "" });
                                continue;
                            }

                            Component comp = db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
                            Lease lease;

                            if (comp.Leases.Any(n => n.BeginDate <= EndDateBoundary && n.EndDate.Value >= EndDateBoundary))
                            {
                                lease = comp.Leases.Where(n => n.BeginDate <= EndDateBoundary && n.EndDate.Value >= EndDateBoundary).Single();
                            }
                            else
                            {
                                lease = comp.Leases.Where(n => n.BeginDate <= CurrentBillDate && n.EndDate >= EndDateBoundary).OrderByDescending(n => n.EndDate).FirstOrDefault();
                            }

                            //if ((lease.MonthlyCharge.Value - MonthlyCost) > 1.00m)
                            //{   
                            //    ErrorMessages.Add(new ErrorMessageRow { SerialNumber = SerialNumber, ErrorType = "Monthly Charge Differs by more than 1.00", Type = comp.Type.Name });
                            //}
                        }
                        else
                        {
                            ErrorMessages.Add(new ErrorMessageRow { SerialNumber = SerialNumber, ErrorType = "Database does not contain a bill Serial Number in the given month", Type = "" });
                        }
                    }
                }
            }

            MemoryStream OutputStream = new MemoryStream();
            TextWriter tw = new StreamWriter(OutputStream);

            foreach (var error in ErrorMessages)
            {
                tw.WriteLine(error.SerialNumber + "," + error.Type + "," + error.ErrorType);
            }

            string fileName = "report.csv";
            string applicationType = "application/octate-stream";

            OutputStream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(OutputStream.ToArray());
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(applicationType);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = fileName };

            return result;
        }

        // PUT api/reconcileagainstinvoice/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/reconcileagainstinvoice/5
        public void Delete(int id)
        {
        }
    }
}
