using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeasingDatabase.Billing;
using System.IO;
using CWSToolkit;

namespace LeasingDatabase.API
{
    public class BillingReportController : ApiController
    {
        // GET api/billingreport/type={Simple or Detailed}&statementDate={date}&FOP={FOP}
        [AuthorizeUser("Admin")]
        public HttpResponseMessage Get(string type, DateTime statementDate, string FOP)
        {
            statementDate = new DateTime(statementDate.Year, statementDate.Month, 15); // Puts the statement date in the middle of the selected month.
            AuleaseEntities db =new AuleaseEntities();
            MemoryStream OutputStream;
            string applicationType, fileName;

            if (type == "Simple")
            {
                BillingReport.SimpleReport(db, statementDate, out OutputStream);
                fileName = statementDate.ToString("MMMyy") + ".txt";
                applicationType = "application/octate-stream";
            }
            else if (type == "Detailed")
            {
                Department dept = FindDepartment(db, FOP);
                BillingReport.DetailedReport(db, statementDate, dept, out OutputStream);
                OutputStream.Position = 0;
                applicationType = "application/xlsx";
                fileName = "AdminBilling-" + statementDate.ToString("MMMyyyy") + ".xlsx";
            }
            else
            {
                throw new Exception("Must specify report type");
            }

            OutputStream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(OutputStream.ToArray());
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(applicationType);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = fileName };

            return result;
        }

        private Department FindDepartment(AuleaseEntities db, string FOP)
        {
            string Fund;
            string Org;
            string Program;

            FOP = FOP.Replace(" ", String.Empty);

            if (FOP.Length == FOPLength.WithDelimeter)
            {

                string[] FOPArray = FOP.Split(new char[] { '-', ' ', ',' });

                Fund = FOPArray[0];
                Org = FOPArray[1];
                Program = FOPArray[2];

            }
            else if (FOP.Length == FOPLength.WithoutDelimiter)
            {
                Fund = FOP.Substring(0, 6);
                Org = FOP.Substring(6, 6);
                Program = FOP.Substring(12, 4);
            }
            else
            {
                throw new Exception("Unknown FOP Type used");
            }

            if (db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
            {
                return db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
            }
            else
            {
                throw new Exception("No Departments found with that FOP");
            }
        }
    }
}
