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
    public class ScanOrder
    {
        public IEnumerable<ScanSystem> orders { get; set; }
        public string SR { get; set; }
    }

    public class ScanSystem
    {
        public int SystemGroupId { get; set; }
        public IEnumerable<ScanComponents> Components { get; set; }
    }

    public class ScanComponents
    {
        public int ComponentId { get; set; }
        public string SerialNumber { get; set; }
        public string LeaseTag { get; set; }
        public string Type { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string StatementName { get; set; }
    }

    public class ScanController : ApiController
    {
        // GET api/scan
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/scan/5
        [AuthorizeUser("Admin", "Users")]
        public ScanOrder Get(string id)
        {
            AuleaseEntities db = new AuleaseEntities();

            string SR;

            IEnumerable<SystemGroup> SystemGroups;

            if (id.ToUpper().StartsWith("SR"))
            {
                // SR Number
                SR = id.ToUpper();
                SystemGroups = db.SystemGroups.Where(n => n.PO.PONumber == id).Distinct();
            }
            else
            {
                // Order Number
                if (db.Components.Any(n => n.OrderNumber == id))
                {
                    SR = db.Components.Where(n => n.OrderNumber == id).SelectMany(n => n.Leases).Select(n => n.SystemGroup).FirstOrDefault().PO.PONumber;
                    SystemGroups = db.Components.Where(n => n.OrderNumber == id).ToList().SelectMany(n => n.Leases).Select(n => n.SystemGroup).Distinct();
                }
                else
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("Invalid Order Number"),
                        ReasonPhrase = "No Order Number found in the database"
                    };
                    throw new HttpResponseException(resp);
                }
            }

            IEnumerable<ScanSystem> orders = SystemGroups.Select(n => new ScanSystem
            {
                SystemGroupId = n.Id,
                Components = n.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Select(o => new ScanComponents
                {
                    ComponentId = o.Component.Id,
                    SerialNumber = o.Component.SerialNumber,
                    LeaseTag = o.Component.LeaseTag,
                    Type = o.Component.Type.Name,
                    Make = o.Component.Make.Name,
                    Model = o.Component.Model.Name,
                    StatementName = o.StatementName
                })
            });

            ScanOrder order = new ScanOrder();
            order.orders = orders;
            order.SR = SR;

            return order;
        }

        

        // POST api/scan
        [AuthorizeUser("Admin", "Users")]
        public void Post([FromBody]IEnumerable<ScanSystem> value)
        {
            AuleaseEntities db = new AuleaseEntities();

            foreach (ScanSystem system in value)
            {
                foreach (var comp in system.Components)
                {
                    Component Component = db.Components.Where(n => n.Id == comp.ComponentId).Single();

                    if (!String.IsNullOrWhiteSpace(comp.SerialNumber)) { Component.SerialNumber = comp.SerialNumber.Trim().ToUpper(); }
                    if (!String.IsNullOrWhiteSpace(comp.LeaseTag)) { Component.LeaseTag = comp.LeaseTag.Trim().ToUpper(); }
                }
            }

            db.SaveChanges();
        }

        // PUT api/scan/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/scan/5
        public void Delete(int id)
        {
        }
    }
}
