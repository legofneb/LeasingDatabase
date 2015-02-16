using aulease.Entities;
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
    public class DatabaseReportController : ApiController
    {
        public HttpResponseMessage Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            MemoryStream OutputStream = new MemoryStream();
            TextWriter tw = new StreamWriter(OutputStream);


            List<Component> components = db.Components.Where(n => n.Leases.FirstOrDefault().MonthlyCharge != null).ToList();

            foreach (var comp in components)
            {
                string componentString = comp.SerialNumber + "," +
                                         comp.LeaseTag + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().StatementName + "," +
                                         comp.Type.Name + "," +
                                         comp.Make.Name + "," +
                                         comp.Model.Name + "," +    
                                         comp.Renewal.ToString() + "," +
                                         comp.OrderNumber + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().Overhead.RateLevel + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().BeginDate.Value.ToString("d") + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().EndDate.Value.ToString("d") + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().MonthlyCharge + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().ContractNumber + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Fund + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Org + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Program + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Name + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.PO.PONumber + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.GID + "," +
                                         comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.PO.PONumber + ",";

                tw.WriteLine(componentString);
            }

            tw.Flush();

            string fileName = "report.csv";
            string applicationType = "application/octate-stream";

            OutputStream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(OutputStream.ToArray());
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(applicationType);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = fileName };

            return result;
        }

    }
}
