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
    public class LeaseReportController : ApiController
    {
        public HttpResponseMessage Post()
        {
            var httpRequest = HttpContext.Current.Request;
            List<string> errorMessages = new List<string>();
            
            var file = httpRequest.Files[0];

            List<string> LeaseTagsInTSMReport = new List<string>();

            using (StreamReader sr = new StreamReader(file.InputStream))
            {
                sr.ReadLine();
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    LeaseTagsInTSMReport.Add(line.Split(',')[1].Substring(0, 7).Trim());
                }
            }


            MemoryStream OutputStream = new MemoryStream();
            TextWriter tw = new StreamWriter(OutputStream);

            AuleaseEntities db = new AuleaseEntities();

            DateTime date = DateTime.Now.AddMonths(-5);
            List<Component> components = db.Components.Where(n => n.Leases.OrderByDescending(o => o.EndDate).Where(o => o.EndDate.HasValue && o.EndDate.Value > date).Count() > 0).Where(n => n.TypeId.HasValue && (n.Type.Name == "CPU" || n.Type.Name == "Laptop")).ToList();

            foreach (var comp in components)
            {
                if (LeaseTagsInTSMReport.Contains(comp.LeaseTag))
                {
                    continue;
                }

                tw.WriteLine(comp.LeaseTag + "," + comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup.User.GID + "," + comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().Department.Name + "," + comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().StatementName + "," + comp.Type.Name);
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
