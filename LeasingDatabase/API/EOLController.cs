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
    public class EOLController : ApiController
    {
        // GET api/eol
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/eol/5
        public IEnumerable<EOLModel> Get(DateTime date)
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<Component> Components = db.Components.Where(n => n.ReturnDate.HasValue && n.ReturnDate.Value.Month == date.Month && n.ReturnDate.Value.Year == date.Year);

            IEnumerable<EOLModel> models = Components.Select(n => new EOLModel
            {
                id = n.Id,
                ShortSerialNumber = n.SerialNumber.Length >= 7 ? n.SerialNumber.Substring(n.SerialNumber.Length - 7, 7) : n.SerialNumber,
                SerialNumber = n.SerialNumber,
                LeaseTag = n.LeaseTag,
                Type = n.Type.Name,
                Make = n.Make.Name,
                Model = n.Model.Name,
                StatementName = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().StatementName,
                DepartmentName = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().Department.Name,
                GID = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().SystemGroup.User.GID,
                EndBillingDate = n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate.Value,
                ReturnDate = n.ReturnDate.Value,
                Damages = n.Damages ?? "",
                Decommissioned = (n.Status.Name == "Ready to Ship")
            }).OrderBy(n => n.DepartmentName).ThenBy(n => n.StatementName).ThenBy(n => n.Type);

            return models;
        }

        public void Post(EOLModel model)
        {
            AuleaseEntities db = new AuleaseEntities();
            Component comp = db.Components.Where(n => n.Id == model.id).Single();
            comp.Damages = model.Damages;

            Status status;
            if (model.Decommissioned)
            {
                status = db.Status.Where(n => n.Name == "Ready to Ship").Single();
            }
            else
            {
                status = db.Status.Where(n => n.Name == "Back in Shop").Single();
            }

            comp.Status = status;

            db.SaveChanges();
        }
    }

    public class EOLModel
    {
        public int id { get; set; }
        public string SerialNumber { get; set; }
        public string LeaseTag { get; set; }
        public string ShortSerialNumber { get; set; }
        public string Type { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string StatementName { get; set; }
        public string GID { get; set; }
        public DateTime EndBillingDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Damages { get; set; }
        public bool Decommissioned { get; set; }

        // Fields used in EOL Report
        public int Term { get; set; }
        public decimal MonthlyCharge { get; set; }
        public string SR { get; set; }
        public string DepartmentName { get; set; }
        public string FOP { get; set; }
        public string RateLevel { get; set; }
    }
}
