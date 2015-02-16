using aulease.Entities;
using CWSToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class ComponentBillingModel
    {
        public int id { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StatementName { get; set; }
        public string ContractNumber { get; set; }
        public string FOP { get; set; }
        public string RateLevel { get; set; }
        public decimal MonthlyCharge { get; set; }
    }

    public class ComponentBillingController : ApiController
    {
        // GET api/componentbilling/5
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<ComponentBillingModel> Get(int id)
        {
            AuleaseEntities db = new AuleaseEntities();

            Component comp = db.Components.Where(n => n.Id == id).Single();

            return comp.Leases.Select(n => new ComponentBillingModel
            {
                id = n.Id,
                BeginDate = n.BeginDate.Value,
                EndDate = n.EndDate.Value,
                StatementName = n.StatementName,
                ContractNumber = n.ContractNumber,
                FOP = n.Department.GetFOP(),
                RateLevel = n.RateLevel,
                MonthlyCharge = n.MonthlyCharge.Value
            });
        }

        // POST api/componentbilling
        [AuthorizeUser("Admin")]
        public void Post([FromBody]IEnumerable<ComponentBillingModel> model)
        {
            AuleaseEntities db = new AuleaseEntities();

            int firstLeaseId = model.Where(n => n.id > 0).FirstOrDefault().id;

            Component comp = db.Leases.Where(n => n.Id == firstLeaseId).Single().Component;

            foreach (var billingComp in model)
            {
                Lease lease = db.Leases.Where(n => n.Id == billingComp.id).FirstOrDefault();
                lease.BeginDate = billingComp.BeginDate;
                lease.EndDate = billingComp.EndDate;
                lease.StatementName = billingComp.StatementName;
                lease.ContractNumber = billingComp.ContractNumber;
                lease.Department = FindDepartment(db, billingComp.FOP);
                lease.MonthlyCharge = billingComp.MonthlyCharge;
            }
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
                throw new Exception("No Department Found");
            }
        }
    }
}
