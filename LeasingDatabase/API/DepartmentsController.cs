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
    public class Departments_JS
    {
        public string DepartmentName { get; set; }
        public string FOP { get; set; }
    }

    public class DepartmentsController : ApiController
    {
        // GET api/departments
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<Departments_JS> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            DateTime date = DateTime.Now.AddMonths(-3);

            var depts = db.Departments.Where(n => (n.Leases.Where(o => o.EndDate > date).Count() > 0)).ToList();

            return depts.Select(n => new Departments_JS
            {
                DepartmentName = n.Name,
                FOP = n.GetFOP()
            });
        }

        // POST api/departments
        [AuthorizeUser("Admin")]
        public void Post([FromBody]Departments_JS dept)
        {
            AuleaseEntities db = new AuleaseEntities();

            var Department = FindDepartment(db, dept.FOP);

            Department.Name = dept.DepartmentName.Trim();
            db.SaveChanges();
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
