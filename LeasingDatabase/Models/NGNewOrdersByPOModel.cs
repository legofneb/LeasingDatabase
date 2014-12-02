using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{

    public class NGNewOrdersByPOModel
    {
        private static IEnumerable<NGNewOrdersByPOModel> GetOrdersFromFilteredPOs(IEnumerable<PO> POs)
        {
            return POs.Select(n => new NGNewOrdersByPOModel
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
        }

        public static IEnumerable<NGNewOrdersByPOModel> GetOrdersFromPOs(IEnumerable<PO> POs)
        {
            return GetOrdersFromFilteredPOs(GetUnBilledPOs(POs));
        }

        public static IEnumerable<NGNewOrdersByPOModel> GetOrdersFromPOs(IQueryable<PO> POs)
        {
            return GetOrdersFromFilteredPOs(GetUnBilledPOs(POs));
        }

        private static IEnumerable<PO> GetUnBilledPOs(IEnumerable<PO> query)
        {
            return query.Where(n => n.SystemGroups.Any(o => o.Leases.Any(p => p.MonthlyCharge == null)) && n.SystemGroups.Any(o => o.PO.PONumber != null))
                                                          .OrderByDescending(n => n.PONumber).ToList();
        }

        private static IEnumerable<PO> GetUnBilledPOs(IQueryable<PO> query)
        {
            return query.Where(n => n.SystemGroups.Any(o => o.Leases.Any(p => p.MonthlyCharge == null)) && n.SystemGroups.Any(o => o.PO.PONumber != null))
                                                          .OrderByDescending(n => n.PONumber).ToList();
        }

        public string SR { get; set; }
        public string Summary { get; set; }

        public IEnumerable<NGConfigurationModel> Configuration { get; set; }
        public IEnumerable<NGOrderSystemGroupModel> SystemGroups { get; set; }
    }

    public  class NGConfigurationModel
    {
        public string Type { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }

    public class NGDepartmentModel
    {
        public string FOP { get; set; }
        public string DepartmentName { get; set; }
    }

    public class NGUserModel
    {
        public string GID { get; set; }
        public string Phone { get; set; }
    }
}