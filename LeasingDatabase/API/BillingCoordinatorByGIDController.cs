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
    public class BillingCoordinatorByGID_JSObject
    {
        public string GID { get; set; }
        public List<string> FOPs { get; set; }
        public bool BillingAccess { get; set; }
    }

    public class BillingCoordinatorByGIDController : ApiController
    {
        // GET api/billingcoordinatorbygid/5
        [AuthorizeUser("Admin", "Users")]
        public BillingCoordinatorByGID_JSObject Get(string GID)
        {
            AuleaseEntities db = new AuleaseEntities();
            Coordinator user;
            if (db.Coordinators.Any(n => n.GID.ToUpper() == GID.ToUpper()))
            {
                user = db.Coordinators.Where(n => n.GID.ToUpper() == GID.ToUpper()).Single();
            }
            else
            {
                return null;
            }

            BillingCoordinatorByGID_JSObject js_object = new BillingCoordinatorByGID_JSObject();
            js_object.GID = user.GID;
            js_object.BillingAccess = user.BillingAccess;
            js_object.FOPs = user.Departments.Select(n => n.GetFOP()).ToList();

            return js_object;
        }

        // POST api/billingcoordinatorbygid
        [AuthorizeUser("Admin")]
        public void Post([FromBody]BillingCoordinatorByGID_JSObject js_object)
        {
            AuleaseEntities db = new AuleaseEntities();
            Coordinator Coordinator = db.Coordinators.Where(n => n.GID == js_object.GID).Single();

            // Set Billing Access
            Coordinator.BillingAccess = js_object.BillingAccess;

            // Add Departments
            List<Department> DepartmentsToAdd = new List<Department>();

            foreach (var dept in js_object.FOPs)
            {
                if (FindDepartmentConnectedToCoordinator(Coordinator.Departments, dept) == null)
                {
                    DepartmentsToAdd.Add(FindDepartment(db, dept));
                }
            }

            foreach (var dept in DepartmentsToAdd)
            {
                Coordinator.Departments.Add(dept);
            }

            // Remove Departments

            List<Department> DepartmentsToRemove = new List<Department>();

            foreach (var dept in Coordinator.Departments)
            {
                bool found = false;

                foreach (var deptString in js_object.FOPs)
                {
                    if (DepartmentMatchesString(dept, deptString))
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    DepartmentsToRemove.Add(dept);
                }
            }

            foreach (var dept in DepartmentsToRemove)
            {
                Coordinator.Departments.Remove(dept);
            }

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
                return null;
            }
        }

        private Department FindDepartmentConnectedToCoordinator(IEnumerable<Department> depts, string FOP)
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

            if (depts.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
            {
                return depts.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
            }
            else
            {
                return null;
            }
        }

        private bool DepartmentExists(IEnumerable<Department> depts, string FOP)
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

            return depts.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program);
        }

        private bool DepartmentMatchesString(Department dept, string FOP)
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

            return (dept.Fund == Fund && dept.Org == Org && dept.Program == Program);
        }
    }
}
