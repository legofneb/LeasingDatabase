using aulease.Entities;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class NewOrdersPreScanController : ApiController
    {
        // GET api/newordersprescan
        public IEnumerable<NGNewOrdersByPOModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<NGNewOrdersByPOModel> Orders = db.POes.Where(n => n.SystemGroups.Any(o => o.Leases.Any(p => p.MonthlyCharge == null)) && n.SystemGroups.Any(o => o.PO.PONumber != null) && n.SystemGroups.Any(o => o.Leases.Any(p => p.Component.SerialNumber == null || p.Component.SerialNumber.Length < 1)))
                                                          .OrderByDescending(n => n.PONumber).ToList()
                .Select(n => new NGNewOrdersByPOModel
                {
                    SR = n.PONumber,
                    Summary = n.SystemGroups.FirstOrDefault().ToString(),
                    Configuration = n.SystemGroups.FirstOrDefault().Leases.Select(o => o.Component).OrderBy(o => o.TypeId).Select(o => new NGConfigurationModel
                    {
                        Type = o.Type != null ? o.Type.Name : null,
                        Make = o.Make != null ? o.Make.Name : null,
                        Model = o.Model != null ? o.Model.Name : null
                    }),
                    SystemGroups = n.SystemGroups.Select(o => new NGOrderSystemGroupModel
                    {
                        StatementName = o.Leases.FirstOrDefault().StatementName,
                        GID = o.User.GID,
                        DepartmentName = o.Leases.FirstOrDefault().Department.Name,
                        FOP = o.Leases.FirstOrDefault().Department.GetFOP(),
                        RateLevel = o.Leases.FirstOrDefault().Overhead != null ? o.Leases.FirstOrDefault().Overhead.RateLevel : null,
                        Term = o.Leases.FirstOrDefault().Overhead != null ? o.Leases.FirstOrDefault().Overhead.Term.ToString() : null,
                        InstallHardware = o.Leases.FirstOrDefault().Component.InstallHardware,
                        InstallSoftware = o.Leases.FirstOrDefault().Component.InstallSoftware,
                        Renewal = o.Leases.FirstOrDefault().Component.Renewal,

                        Phone = o.User.Phone,
                        Room = o.Location.Room,
                        Building = o.Location.Building,
                        Notes = o.Leases.FirstOrDefault().Component.Note,

                        Components = o.Leases.Select(p => new NGComponentModel
                        {
                            SerialNumber = p.Component.SerialNumber,
                            LeaseTag = p.Component.LeaseTag,
                            Type = p.Component.TypeId.HasValue ? p.Component.Type.Name : null
                        }),

                        EOLComponents = o.EOLComponents.Select(p => new NGComponentModel
                        {
                            SerialNumber = p.SerialNumber,
                            LeaseTag = p.LeaseTag,
                            Type = p.TypeId.HasValue ? p.Type.Name : null
                        })
                    })
                });

            return Orders;
        }

        // GET api/newordersprescan/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/newordersprescan
        public void Post([FromBody]string value)
        {
        }

        // PUT api/newordersprescan/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/newordersprescan/5
        public void Delete(int id)
        {
        }
    }
}
