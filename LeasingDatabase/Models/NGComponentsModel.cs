using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
    /// <summary>
    /// Class that represents the collection of SRs and their attributes.
    /// Important to note, this is very different from the NGComponent which describes a single system component.
    /// </summary>
    public class NGComponentsModel
    {
        private static IEnumerable<NGComponentsModel> GetComponentsFromFilteredQuery(IEnumerable<PO> POs)
        {
            return POs.Select(n => new NGComponentsModel
                {
                    id = n.Id,
                    SR = n.PONumber,
                    Summary = n.SystemGroups.FirstOrDefault().ToString(),
                    Configuration = n.SystemGroups.FirstOrDefault().Leases.Select(o => o.Component).Distinct().OrderBy(o => o.TypeId).Select(o => new NGConfigurationModel
                    {
                        Type = o.Type != null ? o.Type.Name : null,
                        Make = o.Make != null ? o.Make.Name : null,
                        Model = o.Model != null ? o.Model.Name : null
                    }),
                    SystemGroups = n.SystemGroups.Distinct().Select(o => new NGPostBillingSystemGroupModel
                    {
                        id = o.Id,
                        GID = o.User.GID,
                        Phone = o.User.Phone,
                        Room = o.User.Location.Room,
                        Building = o.User.Location.Building,

                        OrdererGID = o.Order.User.GID,
                        OrdererPhone = o.Order.User.Phone,
                        OrdererRoom = o.Order.User.Location.Room,
                        OrdererBuilding = o.Order.User.Location.Building,

                        Components = o.Leases.Select(p => p.Component).Distinct().OrderBy(p => p.TypeId).Select(p => new NGPostBillingComponentModel
                        {
                            id = p.Id,
                            Type = p.Type.Name,
                            Make = p.Make.Name,
                            Model = p.Model.Name,
                            SerialNumber = p.SerialNumber,
                            LeaseTag = p.LeaseTag,
                            StatementName = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().StatementName,
                            DepartmentName = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().Department.Name,
                            FOP = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().Department.GetFOP(),
                            RateLevel = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().Overhead.RateLevel,
                            Term = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().Overhead.Term,
                            ContractNumber = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().ContractNumber,
                            Notes = p.Note,
                            OrderNumber = p.OrderNumber,
                            InstallHardware = p.InstallHardware,
                            InstallSoftware = p.InstallSoftware,
                            Renewal = p.Renewal,
                            BeginDate = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().BeginDate.Value,
                            EndDate = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().EndDate.Value,
                            MonthlyCharge = p.Leases.OrderByDescending(r => r.EndDate).FirstOrDefault().MonthlyCharge.Value
                        })
                    })
                });
        }

        public static IEnumerable<NGComponentsModel> GetPostBillingComponents(AuleaseEntities db)
        {
            DateTime CutOffDate = DateTime.Now.AddMonths(-6);

            IQueryable<PO> POs = db.Leases.Where(n => n.MonthlyCharge != null).Select(n => n.SystemGroup).Where(n => n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate > CutOffDate).Select(n => n.PO).Distinct().OrderByDescending(n => n.PONumber);

            return GetComponentsFromFilteredQuery(POs);
        }

        public static IEnumerable<NGComponentsModel> GetPostBillingComponents(AuleaseEntities db, string filteredTerm)
        {
            filteredTerm = filteredTerm.ToUpper();
            List<string> SearchTerms = filteredTerm.Split(' ').ToList();
            DateTime CutOffDate = DateTime.Now.AddMonths(-6);

            IQueryable<Lease> leases = db.Leases.Where(n => n.MonthlyCharge != null).Where(n => n.EndDate > CutOffDate);

            string POSearchTerm = "";
            bool filterByPO = true;

            List<string> itemsToRemove = new List<string>();

            for (int i =0; i < SearchTerms.Count; i ++)
            {
                string searchTerm = SearchTerms[i];
                int term;
                DateTime date;

                if (searchTerm == "UNCONTRACTED")
                {
                    leases = leases.Where(n => n.ContractNumber == null || n.ContractNumber.Length < 1);
                    itemsToRemove.Add(searchTerm);
                }
                else if (db.Users.Any(n => n.GID.ToUpper() == searchTerm))
                {
                    leases = leases.Where(n => n.SystemGroup.User.GID.ToUpper() == searchTerm || n.SystemGroup.Order.SystemGroups.Select(o => o.Order).Where(o => o.User.GID.ToUpper() == searchTerm).Count() > 0);
                    itemsToRemove.Add(searchTerm);
                }
                else if (db.Components.Any(n => n.LeaseTag.ToUpper().Contains(searchTerm)))
                {
                    leases = leases.Where(n => n.Component.LeaseTag.ToUpper().Contains(searchTerm));
                    itemsToRemove.Add(searchTerm);
                }
                else if (searchTerm.StartsWith("SR") && searchTerm.Length <= 7)
                {
                    filterByPO = true;
                    POSearchTerm = searchTerm;
                    itemsToRemove.Add(searchTerm);
                }
                else if (searchTerm.Length == 2 && int.TryParse(searchTerm, out term))
                {
                    leases = leases.Where(n => n.Overhead.Term == term);
                    itemsToRemove.Add(searchTerm);
                }
                else if (DateTime.TryParse(searchTerm, out date))
                {
                    leases = leases.Where(n => (n.BeginDate.HasValue && n.BeginDate.Value.Month == date.Month && n.BeginDate.Value.Year == date.Year) ||
                                               (n.EndDate.HasValue && n.EndDate.Value.Month == date.Month && n.EndDate.Value.Year == date.Year));
                    itemsToRemove.Add(searchTerm);
                }
            }

            foreach (var item in itemsToRemove)
            {
                SearchTerms.Remove(item);
            }

            IQueryable<PO> POs = leases.Select(n => n.SystemGroup).Select(n => n.PO).Distinct();

            if (filterByPO)
            {
                POs = POs.Where(n => n.PONumber.StartsWith(POSearchTerm));
            }

            POs = POs.OrderByDescending(n => n.PONumber);

            IEnumerable<NGComponentsModel> Components = GetComponentsFromFilteredQuery(POs);

            List<NGComponentsModel> FilteredComponents = new List<NGComponentsModel>();

            if (SearchTerms.Count == 0)
            {
                FilteredComponents = Components.ToList();
            }
            else
            {
                foreach (var comp in Components)
                {
                    if (FuzzySearchMatch(comp, SearchTerms))
                    {
                        FilteredComponents.Add(comp);
                    }
                }
            }

            return FilteredComponents.OrderByDescending(n => n.SR);
        }

        private static bool FuzzySearchMatch(NGComponentsModel comp, List<string> SearchTerms)
        {
            bool match = false;

            for (int i = 0; i < SearchTerms.Count; i++)
            {
                string singleSearchString = SearchTerms[i].ToUpper();
                                
                match = FuzzyStringMatch(comp, singleSearchString.ToUpper());

                if (match) { return match; }
            }

            return match;
        }

        private static bool FuzzyStringMatch(NGComponentsModel comp, string singleSearchString)
        {
            foreach (var systemGroup in comp.SystemGroups)
            {
                if (FieldMatchesText(systemGroup.Building, singleSearchString) ||
                    FieldMatchesText(systemGroup.OrdererBuilding, singleSearchString) ||
                    FieldMatchesText(systemGroup.OrdererRoom, singleSearchString) ||
                    FieldMatchesText(systemGroup.Room, singleSearchString))
                {
                    return true;
                }

                foreach (var component in systemGroup.Components)
                {
                    if (FieldContainsText(component.ContractNumber, singleSearchString) ||
                        FieldContainsText(component.DepartmentName, singleSearchString) ||
                        FieldContainsText(component.FOP, singleSearchString) ||
                        FieldMatchesText(component.Make, singleSearchString) ||
                        FieldMatchesText(component.Model, singleSearchString) ||
                        FieldContainsText(component.OrderNumber, singleSearchString) ||
                        FieldMatchesText(component.RateLevel, singleSearchString) ||
                        FieldContainsText(component.SerialNumber, singleSearchString) ||
                        FieldContainsText(component.StatementName, singleSearchString) ||
                        FieldMatchesText(component.Type, singleSearchString))
                    {
                        return true;
                    }
                }

                if (systemGroup.EOLComponents != null)
                {
                    foreach (var component in systemGroup.EOLComponents)
                    {
                        if (FieldMatchesText(component.LeaseTag, singleSearchString) ||
                            FieldMatchesText(component.SerialNumber, singleSearchString))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool FieldContainsText(string p, string singleSearchString)
        {
            if (String.IsNullOrEmpty(p))
            {
                return false;
            }

            return p.ToUpper().Contains(singleSearchString);
        }

        private static bool FieldMatchesText(string p, string singleSearchString)
        {
            if (String.IsNullOrEmpty(p))
            {
                return false;
            }

            return p.ToUpper() == singleSearchString;
        }

        private static bool FuzzyIntMatch(NGComponentsModel comp, int number)
        {
            foreach (var component in comp.SystemGroups.SelectMany(n => n.Components))
            {
                if (component.Term == number)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool FuzzyDateMatch(NGComponentsModel comp, DateTime date)
        {
            foreach (var component in comp.SystemGroups.SelectMany(n=> n.Components))
            {
                if (date.Month == component.BeginDate.Month && date.Year == component.BeginDate.Year)
                {
                    return true;
                }
                else if (date.Month == component.EndDate.Month && date.Year == component.EndDate.Year)
                {
                    return true;
                }
            }
            return false;
        }

        public int id { get; set; }
        public string SR { get; set; }
        public string Summary { get; set; }

        public IEnumerable<NGConfigurationModel> Configuration { get; set; }
        public IEnumerable<NGPostBillingSystemGroupModel> SystemGroups { get; set; }
    }

    public class NGPostBillingSystemGroupModel
    {
        public int id { get; set; }
        public string GID { get; set; }

        public string Phone { get; set; }
        public string Room { get; set; }
        public string Building { get; set; }

        public string OrdererGID { get; set; }
        public string OrdererBuilding { get; set; }
        public string OrdererRoom { get; set; }
        public string OrdererPhone { get; set; }

        public IEnumerable<NGPostBillingComponentModel> Components { get; set; }
        public IEnumerable<NGEOLComponentModel> EOLComponents { get; set; }
    }

    public class NGPostBillingComponentModel
    {
        public int id { get; set; }

        public string Type { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }

        public string SerialNumber { get; set; }
        public string LeaseTag { get; set; }
        public string StatementName { get; set; }
        public string DepartmentName { get; set; }
        public string FOP { get; set; }
        public string RateLevel { get; set; }
        public int Term { get; set; }
        public string ContractNumber { get; set; }
        public string Notes { get; set; }

        


        public string OrderNumber { get; set; }

        public bool InstallHardware { get; set; }
        public bool InstallSoftware { get; set; }
        public bool Renewal { get; set; }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal MonthlyCharge { get; set; }
    }
}