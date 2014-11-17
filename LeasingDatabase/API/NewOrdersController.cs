using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using aulease.Entities;
using LeasingDatabase.Models;

namespace LeasingDatabase.API
{
    public class NewOrdersController : ApiController
    {
        // GET api/neworders
        public IEnumerable<NGNewOrdersModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<NGNewOrdersModel> Orders = db.Orders.Where(n => n.SystemGroups.Any(o => o.Leases.Any(p => p.MonthlyCharge == null)) && n.SystemGroups.Any(o => o.PO.PONumber == null))
                                                          .OrderByDescending(n => n.Date).ToList()
                .Select(n => new NGNewOrdersModel
            {
                id = n.Id,
                Date = n.Date.ToString("d"),
                OrdererGID = n.User.GID,
                OrdererBuilding = n.User.Location.Building,
                OrdererRoom = n.User.Location.Room,
                OrdererPhone = n.User.Phone,
                Configuration = n.SystemGroups.FirstOrDefault().Leases.Select(o => o.Component).OrderBy(o => o.TypeId).Select(o => new NGConfigurationModel {
                    Type = o.Type != null ? o.Type.Name:null,
                    Make = o.Make != null ? o.Make.Name : null,
                    Model = o.Model != null ? o.Model.Name : null
                }),
                Components = n.SystemGroups.Select(o => new NGOrderSystemGroupModel 
                {
                    StatementName = o.Leases.FirstOrDefault().StatementName,
                    GID = o.User.GID,
                    DepartmentName = o.Leases.FirstOrDefault().Department.Name,
                    FOP = o.Leases.FirstOrDefault().Department.GetFOP(),
                    RateLevel = o.Leases.FirstOrDefault().Overhead != null ? o.Leases.FirstOrDefault().Overhead.RateLevel : null,
                    Term = o.Leases.FirstOrDefault().Overhead != null ? o.Leases.FirstOrDefault().Overhead.Term.ToString() : null,
                    InstallHardware = o.Leases.FirstOrDefault().Component.InstallHardware,
                    InstallSoftware = o.Leases.FirstOrDefault().Component.InstallSoftware,
                    Renewal = o.Leases.FirstOrDefault().Component.Renewal
                }),
                Summary = n.SystemGroups.FirstOrDefault().ToString(),
                Notes = n.Note
            });

            return Orders;
        }

        // GET api/neworders/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/neworders
        public NGNewOrdersModel Post([FromBody]NGNewOrdersModel order)
        {

            return order;
        }

        // PUT api/neworders/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/neworders/5
        public void Delete(int id)
        {
        }
    }
}
