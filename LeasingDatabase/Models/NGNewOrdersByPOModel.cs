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
                    id = n.Id,
                    SR = n.PONumber,
                    Summary = n.SystemGroups.FirstOrDefault().ToString(),
                    Configuration = n.SystemGroups.FirstOrDefault().Leases.Select(o => o.Component).OrderBy(o => o.TypeId).Select(o => new NGConfigurationModel
                    {
                        Type = o.Type != null ? o.Type.Name : null,
                        Make = o.Make != null ? o.Make.Name : null,
                        Model = o.Model != null ? o.Model.Name : null
                    }),
                    SystemGroups = n.SystemGroups.Select(o => new NGOrderSystemGroupByPOModel
                    {
                        id = o.Id,
                        Date = o.Order.Date.ToString("d"),
                        OrderNumber = o.Leases.FirstOrDefault().Component.OrderNumber,
                        OrdererGID = o.Order.User.GID,
                        OrdererBuilding = o.Order.User.Location.Building,
                        OrdererRoom = o.Order.User.Location.Room,
                        OrdererPhone = o.Order.User.Phone,

                        StatementName = o.Leases.FirstOrDefault().StatementName,
                        GID = o.User.GID,
                        DepartmentName = o.Leases.FirstOrDefault().Department.Name,
                        FOP = o.Leases.FirstOrDefault().Department.GetFOP(),
                        RateLevel = o.Leases.FirstOrDefault().Overhead != null ? o.Leases.FirstOrDefault().Overhead.RateLevel : null,
                        Term = o.Leases.FirstOrDefault().Overhead != null ? o.Leases.FirstOrDefault().Overhead.Term : (int?)null,
                        InstallHardware = o.Leases.FirstOrDefault().Component.InstallHardware,
                        InstallSoftware = o.Leases.FirstOrDefault().Component.InstallSoftware,
                        Renewal = o.Leases.FirstOrDefault().Component.Renewal,

                        Phone = o.User.Phone,
                        Room = o.Location.Room,
                        Building = o.Location.Building,
                        Notes = o.Leases.FirstOrDefault().Component.Note,

                        Components = o.Leases.Select(p => p.Component).Distinct().OrderByDescending(p => p.TypeId).Select(p => new NGComponentModel
                        {
                            SerialNumber = p.SerialNumber,
                            LeaseTag = p.LeaseTag,
                        }),

                        EOLComponents = o.EOLComponents.Select(p => new NGEOLComponentModel
                        {
                            SerialNumber = p.SerialNumber,
                            LeaseTag = p.LeaseTag,
                            Type = p.TypeId.HasValue ? p.Type.Name : null,
                            Make = p.MakeId.HasValue ? p.Make.Name : null,
                            Model = p.ModelId.HasValue ? p.Model.Name : null
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

        public int id { get; set; }
        public string SR { get; set; }
        public string Summary { get; set; }

        public IEnumerable<NGConfigurationModel> Configuration { get; set; }
        public IEnumerable<NGOrderSystemGroupByPOModel> SystemGroups { get; set; }
    }

    public class NGOrderSystemGroupByPOModel
    {
        public int id { get; set; }
        public string Date { get; set; }
        public string OrderNumber { get; set; }
        public string OrdererGID { get; set; }
        public string OrdererBuilding { get; set; }
        public string OrdererRoom { get; set; }
        public string OrdererPhone { get; set; }

        public string StatementName { get; set; }
        public string GID { get; set; }
        public string DepartmentName { get; set; }
        public string FOP { get; set; }
        public string RateLevel { get; set; }
        public int? Term { get; set; }
        public bool InstallHardware { get; set; }
        public bool InstallSoftware { get; set; }
        public bool Renewal { get; set; }
        public string Phone { get; set; }
        public string Room { get; set; }
        public string Building { get; set; }
        public IEnumerable<NGComponentModel> Components { get; set; }
        public IEnumerable<NGEOLComponentModel> EOLComponents { get; set; }

        public string Notes { get; set; }
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