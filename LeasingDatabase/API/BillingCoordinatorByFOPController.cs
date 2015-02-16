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

    public class BillingCoordinatorByFOP_JSObject
    {
        public List<BillingCoordinators> Coordinators;
        public string FOP;
    }

    public class BillingCoordinators
    {
        public string GID { get; set; }
        public bool BillingAccess { get; set; }
    }

    public class BillingCoordinatorByFOPController : ApiController
    {
        // GET api/billingcoordinatorbyfop/5
        [AuthorizeUser("Admin", "Users")]
        public BillingCoordinatorByFOP_JSObject Get(string FOP)
        {
            AuleaseEntities db = new AuleaseEntities();

            Department dept = FindDepartment(db, FOP);

            BillingCoordinatorByFOP_JSObject js_object = new BillingCoordinatorByFOP_JSObject();
            js_object.FOP = dept.GetFOP();
            js_object.Coordinators = dept.Coordinators.Select(n => new BillingCoordinators
            {
                GID = n.GID,
                BillingAccess = n.BillingAccess
            }).ToList();

            return js_object;
        }

        // POST api/billingcoordinatorbyfop
        [AuthorizeUser("Admin")]
        public void Post([FromBody]BillingCoordinatorByFOP_JSObject js_object)
        {
            AuleaseEntities db = new AuleaseEntities();

            Department dept = FindDepartment(db, js_object.FOP);

            List<BillingCoordinators> CoordinatorsToAdd = new List<BillingCoordinators>();

            // Add Coordinators

            foreach (var obj in js_object.Coordinators)
            {
                if (!ContainsGID(dept.Coordinators, obj.GID))
                {
                    CoordinatorsToAdd.Add(obj);
                }
                else
                {
                    // Modify Coordinators Found
                    Coordinator coordinator = FindOrCreateCoordinator(db, obj);
                    coordinator.BillingAccess = obj.BillingAccess;
                }
            }

            foreach (var coordinatorToAdd in CoordinatorsToAdd)
            {
                Coordinator coordinator = FindOrCreateCoordinator(db, coordinatorToAdd);
                coordinator.BillingAccess = coordinatorToAdd.BillingAccess;
                dept.Coordinators.Add(coordinator);
            }

            // Remove Coordinators

            List<Coordinator> CoordinatorsToRemove = new List<Coordinator>();

            foreach (var coordinator in dept.Coordinators)
            {
                bool found = false;

                foreach (var billingCoordinator in js_object.Coordinators)
                {
                    if (coordinator.GID.ToUpper() == billingCoordinator.GID.ToUpper())
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    CoordinatorsToRemove.Add(coordinator);
                }
            }

            foreach (var coordinator in CoordinatorsToRemove)
            {
                dept.Coordinators.Remove(coordinator);
            }

            db.SaveChanges();
        }

        private bool ContainsGID(IEnumerable<Coordinator> coordinators, string GID)
        {
            return coordinators.Select(n => n.GID.ToUpper()).Contains(GID.ToUpper());
        }

        private Coordinator FindOrCreateCoordinator(AuleaseEntities db, BillingCoordinators coordinator)
        {
            if (db.Coordinators.Any(n => n.GID.ToUpper() == coordinator.GID.ToUpper()))
            {
                return db.Coordinators.Where(n => n.GID.ToUpper() == coordinator.GID.ToUpper()).Single();
            }
            else
            {
                return new Coordinator { GID = coordinator.GID };
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
                return null;
            }
        }
    }
}
