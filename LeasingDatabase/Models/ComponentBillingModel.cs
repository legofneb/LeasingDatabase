using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
    public class ComponentBillingModel
    {
        public int id { get; set; }
        public long BeginDate { get; set; }
        public long EndDate { get; set; }
        public string StatementName { get; set; }
        public string ContractNumber { get; set; }
        public string FOP { get; set; }
        public string RateLevel { get; set; }
        public decimal MonthlyCharge { get; set; }
    }
}